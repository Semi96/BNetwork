using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Stripe;
using BnetworkManagement.Services;
using BnetworkManagement.Models.AccountViewModels;
using BnetworkManagement.Models;
using Microsoft.AspNetCore.Identity;
using BnetworkManagement.Data;
using Microsoft.Extensions.Logging;
using BnetworkManagement.Models.BusinessViewModels;
using Coinbase;
using Coinbase.Wallet;
using Coinbase.Models.Transaction;
using System.Threading;
using BnetworkManagement.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BnetworkManagement.Controllers
{

    public class PaymentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        static AsyncLocker<string> userLock = new AsyncLocker<string>();
        static HashSet<string> withdrawSet = new HashSet<string>();


        public PaymentController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }


        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> Checkout(CheckoutViewModel model, string callback)
        //{
        //    return View(model);
        //}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout(int mh, string discount)
        {
            var MHrate =_context.AvailableCapacity.Find(1).UnitPrice;
            var user = await _userManager.GetUserAsync(User);           

                CheckoutViewModel model = new CheckoutViewModel
                {
                    MegaHashPurchased = mh,
                    MegaHashPriceRate = MHrate, // get value from database 
                    NoTaxPurchasePrice = MHrate * mh,
                    Cryptocurrency = Currencies.Ethereum
                };

                // MUST explicity load the user Wallet
                _context.Entry(user).Reference(s => s.Wallet).Load();
                var wallet = user.Wallet;

                if (wallet != null)
                {
                    model.WalletPublicKey = wallet.WalletKey;
                }

            if (discount != null)
            {
                
                var discountCode = _context.DiscountCodes.Where(p => p.Code == discount);
                if (discountCode != null)
                {
                    // check that it is not the own users discount code
                    _context.Entry(user).Reference(s => s.AffiliateCode).Load();
                    var UserAffiliateCode = user.AffiliateCode.Code;
                    if (discount == UserAffiliateCode)
                    {
                        // do not apply discount
                    }
                    else if (discountCode.Last().DiscountType == DiscountType.Percent)
                    {
                        model.DiscountCode = discount;
                        model.DiscountApplied = true;
                        model.NoTaxPurchasePrice = model.NoTaxPurchasePrice * (decimal)(1 - discountCode.Last().DiscountAmount/100);
                    }
                    else if (discountCode.Last().DiscountType == DiscountType.DollarAmount)
                    {
                        model.DiscountCode = discount;
                        model.DiscountApplied = true;
                        model.NoTaxPurchasePrice -= discountCode.Last().DiscountAmount;
                    }
                }
            }

            // IMPLEMENT TAX BASED ON LOCATION
            _context.Entry(user).Reference(s => s.UserAddress).Load();
            var userAddress = user.UserAddress.Country;
            if (userAddress != null)
            {

                if (user.UserAddress.Country.Trim().ToUpper().Equals("CANADA") || user.UserAddress.Country.Equals("CAN"))
                {
                    model.PurchaseTaxRate = 0.13M;
                    model.NetPurchaseTax = model.PurchaseTaxRate * model.NoTaxPurchasePrice;
                    model.TotalPurchasePrice = model.NoTaxPurchasePrice + model.NetPurchaseTax;
                }
                else
                {
                    model.PurchaseTaxRate = 0;
                    model.NetPurchaseTax = 0;
                    model.TotalPurchasePrice = model.NoTaxPurchasePrice;
                }
            }


                return View(model);
        }


       public ActionResult CheckoutComplete()
        {

            return View("CheckoutComplete");
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> ApplyDiscountCode(CheckoutViewModel model)
        //{
        //    var code = model.DiscountCode;
        //}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
            {
                return View(Index());
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{user.Id}'.");
            }

                // explicity load the user Wallet
                _context.Entry(user).Reference(s => s.Wallet).Load();
                var wallet = user.Wallet;

                if (wallet == null && model.WalletPublicKey != null)
                {
                    // user does not have an associated Wallet but will create one now
                    user.Wallet = new UserWallet
                    {
                        Currency = model.Cryptocurrency,
                        WalletKey = model.WalletPublicKey
                    };
                }

                // pull BatchOrder information from DB
                var batch = _context.MiningInventory.LastOrDefault();
                if (batch == null || batch.Status != BatchStatus.Waiting)
                {
                    var newBatch = new MiningInventory
                    {
                        Currency = model.Cryptocurrency,
                        MHAmount = 0,
                        Status = BatchStatus.Waiting
                    };

                    _context.Add(newBatch);
                    batch = newBatch;
                    //await _context.SaveChangesAsync();
                    //batch = _context.MiningInventory.LastOrDefault();
                }

                batch.MHAmount += model.MegaHashPurchased;
                if (batch.MHAmount > 1750)
                {
                    // close status of previous batch
                    batch.Status = BatchStatus.Pending;
                    batch.OrderDate = DateTime.Now;

                    // create new batch
                    var newBatch = new MiningInventory
                    {
                        Currency = Currencies.Ethereum,
                        MHAmount = 0,
                        Status = BatchStatus.Waiting
                    };
                    _context.Add(newBatch);
                }

                DiscountCode discount = null;
                if (model.DiscountCode != null)
                {
                    var discountCode = _context.DiscountCodes.Where(p => p.Code == model.DiscountCode);
                    discount = discountCode.LastOrDefault();
                    if (discount != null)
                    {
                        ++discount.NumTimesUsed;
                        _context.DiscountCodes.Update(discount);
                    }
                }

                var order = new RentalPurchaseContract
                {
                    AppUser = user,
                    PurchaseDate = DateTime.Now,
                    MHAmount = model.MegaHashPurchased,
                    UnitPrice = model.MegaHashPriceRate,
                    DiscountCode = discount,
                    Taxrate = (double)model.PurchaseTaxRate,
                    PaymentToken = model.PaymentToken,
                    Currency = model.Cryptocurrency,
                    Batch = batch
                };

                _context.Add(order);
                //user.Status = UserStatus.Pending;


                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Charge(string walletKey, string stripeEmail, string stripeToken, CheckoutViewModel model)
        {
            using (var transaction = _context.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    try
                    {
                        var cap = _context.AvailableCapacity.Find(1);
                        cap.TotalMegaHashAvailable -= model.MegaHashPurchased;
                        _context.Update(cap);
                        _context.SaveChanges();

                        model.PaymentToken = stripeToken;
                        model.WalletPublicKey = walletKey;

                        var customers = new StripeCustomerService();
                        var charges = new StripeChargeService();

                        var customer = customers.Create(new StripeCustomerCreateOptions
                        {
                            Email = stripeEmail,
                            SourceToken = stripeToken
                        });

                        var charge = charges.Create(new StripeChargeCreateOptions
                        {
                            Amount = (int) (model.TotalPurchasePrice * 100), // cents
                            Description = "BNETWORK MINING CONTRACT",
                            ReceiptEmail = stripeEmail,
                            Currency = "usd",
                            CustomerId = customer.Id
                        });

                        if (charge.Paid || charge.Status == "succeeded")
                        {
                            JsonResult result = (JsonResult) await Checkout(model);

                            _logger.LogInformation("User Purchase Complete");
                            _context.SaveChanges();
                            transaction.Commit();
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
                    }
                    catch (Exception e)
                    {
                        return Json(new { success = false });
                    }
                }
        }


        public async Task<IActionResult> RequestWithdraw(double requestAmount, Currencies currency)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{user.Id}'.");
                //return Json(new { success = false });
            }
            //using (await userLock.LockAsync(user.Id))
            //{
            bool userRequestExists = withdrawSet.Contains(user.Id);
            if (userRequestExists)
            {
                return null;
            }
            else
            {
                try
                {
                    withdrawSet.Add(user.Id);
                    if (requestAmount < 0.05)
                    {
                        return Json(new { success = false, responseText = "withdrawal request failed; the minimum withdrawal amount is 0.05 ETH." });

                    }
                    if (!await CheckUserBalance(requestAmount, user))
                    {
                        // error; you do not have sufficient funds to support this withdrawal
                        return Json(new { success = false, responseText = "withdrawal request failed; you do not have sufficient funds for this withdrawal." });
                    }
                    else if (!await CheckWithdrawTime(user))
                    {
                        // error; you may only withdraw once every 24h
                        return Json(new { success = false, responseText = "withdrawal request failed; you may only withdraw once every 24 hours." });
                    }
                    else
                    {
                        var SendCurrency = "";
                        switch (currency)
                        {
                            case Currencies.Ethereum:
                                SendCurrency = "ETH";
                                break;
                            //case Currencies.Bitcoin:
                            //    SendCurrency = "BTC";
                            //    break;
                            //case Currencies.Monero:
                            //    SendCurrency = "XMR";
                            //    break;
                            default:
                                return Json(new { success = false, responseText = "withdrawal request failed- unable to resolve currency; please contact us for more information" });
                        }

                        var client = new Client("", "");

                        var fee = 0.0005; // get current fee in realtime
                        var deliverAmount = requestAmount - fee;
                        //if (!await CheckCoinbaseBalance(requestAmount, client))
                        //{
                        //    // error; we seem to not have enough funds in our wallet to support this withdraw
                        //    withdrawSet.Remove(user.Id);
                        //    return Json(new { success = false, responseText = "withdrawal request failed- unable to transfer; please contact us for more information." });
                        //}

                        var withdrawal = new UserTransaction
                        {
                            AppUser = user,
                            CryptoRequested = requestAmount,
                            CryptoDelivered = deliverAmount,
                            FeePaid = fee,
                            Date = DateTime.Now
                        };
                        _context.Add(withdrawal);
                        // call wallet API to send funds-- CAUTION: Executing SendMoney() will send real funds to designated address
                        //var transaction = client.SendMoney("", new TransactionSendModel
                        //{
                        //    To = user.Wallet.WalletKey.ToString(),
                        //    Amount = (decimal) (deliverAmount),
                        //    Currency = "ETH",
                        //    Description = "Withdrawal from Bnetwork Mining.",

                        //});
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, responseText = "withdrawal was successful" });
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = true, responseText = "withdrawal request failed" });
                }
                finally
                {
                    withdrawSet.Remove(user.Id);
                }
                //}
            }
        }

        public async Task<bool> CheckCoinbaseBalance(double requestAmount, Client client)
        {
            //var client = new Client("", "");
            var accountBalance = client.GetAccount("").Balance.Amount;

            if ((double) accountBalance - requestAmount >= 0.001)
            {
                return true;
            }
            else
            {
                // may not have enough funds to pay for fee of the transfer
                // must notify Bnet admins to resolve this error
                return false;
            }

        }


        public async Task<bool> CheckWithdrawTime(ApplicationUser user)
        {
            var withdrawals = _context.UserTransactions.Where(p => p.AppUser == user);
            var lastWithdraw = withdrawals.LastOrDefault();
            if (lastWithdraw == null)
            {
                return true;
            }
            else
            {
                var lastWithdrawTime = lastWithdraw.Date;
                return DateTime.Compare(DateTime.Now, lastWithdrawTime.AddDays(1)) >= 0;
            }
        }

        public async Task<bool> CheckUserBalance(double requestAmount, ApplicationUser user)
        {
            // derive User Balance: Sum of MiningContract Accredits - Sum of User Withdrawals

            var CryptoAccredits = _context.MiningContractProgress.Where(p => p.AppUser == user);
            var TotalAccredited = 0.0;
            foreach (var accredit in CryptoAccredits)
            {
                TotalAccredited += accredit.CryptoMined;
            }

            var UserWithdrawals = _context.UserTransactions.Where(p => p.AppUser == user);
            var TotalWithdrawn = 0.0;
            foreach (var withdrawal in UserWithdrawals)
            {
                TotalWithdrawn += withdrawal.CryptoRequested;
            }

            var UserBalance = TotalAccredited - TotalWithdrawn;

            if (UserBalance - requestAmount < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void RejectChanges()
        {   // can only remove items from a List when iterating in reverse
            foreach (var entry in _context.ChangeTracker.Entries().Reverse())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }
    }
}
