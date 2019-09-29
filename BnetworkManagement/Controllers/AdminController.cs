using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BnetworkManagement.Data;
using BnetworkManagement.Extensions;
using BnetworkManagement.Models;
using BnetworkManagement.Models.AdminViewModels;
using BnetworkManagement.Models.ViewModels;
using BnetworkManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BnetworkManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public int PageSize = 2;
        private readonly IEmailSender _emailSender;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult Messages()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getMessages()
        {
        

            // var userId = user.Id;

            var contextData = _context.Messages.Select(a => new { a.MessageId,a.MessageString, Status= a.Status.ToString(), StartDate = a.StartDate.ToString("ddd, dd MMM yyyy HH':'mm':'ss "), EndDate = a.EndDate.ToString("ddd, dd MMM yyyy HH':'mm':'ss "), a.PostedBy }).ToList();





            var dataData = new { data = contextData };
            var json = JsonConvert.SerializeObject(dataData);

            return Json(dataData);

        }
        [HttpPost]
        public JsonResult getOneMessage(int messageId)
        {
           

            // var userId = user.Id;

            var contextData = _context.Messages.Where(x => x.MessageId == messageId).Select(a => new { a.MessageId, a.MessageString, Status = a.Status, StartDate = a.StartDate, EndDate = a.EndDate, a.PostedBy }).ToList();





            var dataData = new { data = contextData };
            var json = JsonConvert.SerializeObject(dataData);

            return Json(dataData);

        }
        [HttpPost]
        public IActionResult updateMessage( Messages editMessage)
        {

            //var contextData = _context.Messages.Where(x => x.MessageId == messageId).Select(a => new { a.MessageId, a.MessageString, Status = a.Status, StartDate = a.StartDate, EndDate = a.EndDate, a.PostedBy }).ToList();


           



            _context.Messages.Update(editMessage);
                _context.SaveChanges();


            return Redirect("Messages");
            
        }
        [HttpPost]
        public IActionResult removeMessage(int messageId)
        {

            //var contextData = _context.Messages.Where(x => x.MessageId == messageId).Select(a => new { a.MessageId, a.MessageString, Status = a.Status, StartDate = a.StartDate, EndDate = a.EndDate, a.PostedBy }).ToList();

            var newMessage = new Messages
            {
                MessageId = messageId
            };



            
            _context.Messages.Attach(newMessage);
            _context.Messages.Remove(newMessage);
            _context.SaveChanges();


            return View("Messages");

        }

        [HttpPost]
        public IActionResult addMessage(Messages message)
        {
            try
            {


                var newMessage = new Messages
                {
                   
                    MessageString = message.MessageString,
                    Status = message.Status,
                    StartDate = message.StartDate,
                    EndDate = message.StartDate,
                    PostedBy = message.PostedBy

                };

                _context.Add(newMessage); ;
                _context.SaveChanges();
                return View("Messages");

            }
            catch (Exception e)
            {
                return View("Messages");
            }
        }
        public async Task<IActionResult> Index(string userStatus,string currentFilter, int? page)
        {
            ViewData["StatusFilter"] = userStatus;
            if ((userStatus!=null)){
                page = 1;
            }
            else
            {
                userStatus = currentFilter;
            }
            switch (userStatus)
            {
                case "0":
                    ViewData["StatusFilter"] = "NoStatus";
                    break;
                case "1":
                    ViewData["StatusFilter"] = "Activated";
                    break;
                case "2":
                    ViewData["StatusFilter"] = "Pending";
                    break;
                case "3":
                    ViewData["StatusFilter"] = "Mining";
                    break;
                case "4":
                    ViewData["StatusFilter"] = "StppedMining";
                    break;
                case "5":
                    ViewData["StatusFilter"] = "Banned";
                    break;
                default:
                    ViewData["StatusFilter"] = "NoStatus";
                    break;

            }
            var appUsers = from u in _context.AppUsers select u;
            if (!String.IsNullOrEmpty(userStatus))
            {
                appUsers = appUsers.Where(u => (u.Status.ToString()) == ViewData["StatusFilter"].ToString());
            }
            int pageSize = 2;
            return View(await PaginatedList<ApplicationUser>.CreateAsync(appUsers.AsNoTracking(), page ?? 1, pageSize));
            //return View(await appUsers.ToListAsync());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RPCs()
        {
            var rpcs = _context.RentalPurchaseContract.Include(p => p.AppUser).Include(p => p.Batch);

            return View(rpcs.AsEnumerable());
        }

        [HttpGet]
        public IActionResult EditRPCs(int Id)
        {
            RentalPurchaseContract contract = _context.RentalPurchaseContract.Include(p => p.AppUser).Include(p => p.Batch).FirstOrDefault(u => u.TransactionId == Id);

            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> EditRPCs(RentalPurchaseContract model)
        {
            if (ModelState.IsValid)
            {
                var rpc = _context.RentalPurchaseContract.Where(p => p.TransactionId == model.TransactionId).FirstOrDefault();

                rpc.PurchaseDate = model.PurchaseDate;
                rpc.MHAmount = model.MHAmount;
                rpc.UnitPrice = model.UnitPrice;
                rpc.DiscountCode = model.DiscountCode;
                rpc.Taxrate = model.Taxrate;
                rpc.PaymentToken = model.PaymentToken;
                rpc.Currency = model.Currency;

                var user = _context.AppUsers.Where(p => p.Id == model.AppUser.Id).FirstOrDefault();
                var batch = _context.MiningInventory.Where(p => p.BatchId == model.Batch.BatchId).FirstOrDefault();

                rpc.AppUser = user;
                rpc.Batch = batch;

                _context.Update(rpc);
                await _context.SaveChangesAsync();

                StatusMessage = "RPC successfully changed"; // wont work unless implemented in view
                return RedirectToAction(nameof(RPCs));
            }
            return View(model);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Batches()
        {
            var batches = _context.MiningInventory.AsEnumerable();

            return View(batches);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AvailableCapacity()
        {
            var capacity = _context.AvailableCapacity.FirstOrDefault();

            return View(capacity);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Accredits()
        {
            var accredits = _context.MiningContractProgress.Include(p => p.AppUser).Include(p => p.RentalPurchaseContract);
            return View(accredits.AsEnumerable());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Withdrawals()
        {
            var withdrawals = _context.UserTransactions.Include(p => p.AppUser);
            return View(withdrawals.AsEnumerable());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Accounting()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DiscountCodes()
        {
            var batches = _context.DiscountCodes.AsEnumerable();

            return View(batches);
        }

        public IActionResult List(int listPage = 1)
            => View(new UsersListViewModel
            {
                Users = _context.Users.OrderBy(u => u.Id).Skip((listPage - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = listPage,
                    ItemsPerPage = PageSize,
                    TotalItems = _context.Users.Count()
                }
            });

        //public ViewResult List(int? userStatus, int listPage = 1)
        //           => View(new UsersListViewModel
        //           {
        //               Users = _context.Users
        //               .Where(u => userStatus == null || (int)u.Status == userStatus)
        //               .OrderBy(u => u.Id).Skip((listPage - 1) * PageSize).Take(PageSize),
        //               PagingInfo = new PagingInfo
        //               {
        //                   CurrentPage = listPage,
        //                   ItemsPerPage = PageSize,
        //                   TotalItems = _context.Users.Count()
        //               },
        //               CurrentStatus = userStatus
        //           });
        //public IActionResult List(int displayStatus, int listPage = 1)
        //{
        //    ViewData["StatusFilter"] = displayStatus;
        //    var UsersToDisplay = new UsersListViewModel {
        //        Users = from u in _context.AppUsers select u,
        //        PagingInfo = new PagingInfo
        //        {
        //            CurrentPage = listPage,
        //            ItemsPerPage = PageSize,
        //            TotalItems = _context.Users.Count()
        //        }
        //    };
        //    if (displayStatus != 10)
        //    {
        //        UsersToDisplay.CurrentStatus = displayStatus;
        //        UsersToDisplay = new UsersListViewModel
        //        {
        //            Users =  from u in _context.AppUsers select u,
        //            PagingInfo = new PagingInfo
        //            {
        //                CurrentPage = listPage,
        //                ItemsPerPage = PageSize,
        //                TotalItems = _context.Users.Count()
        //            }
        //        };
        //        //Users = Users.Where(u => (int)u.Status == userStatus);
        //        return View(UsersToDisplay);


        //    }
        //    return View();
        //}

        public async Task<IActionResult> Details(string Id)
        {
            var Appuser =  _context.AppUsers.Include(u => u.UserAddress)
                .AsNoTracking()
            .FirstOrDefault(u => u.Id == Id);
            return View(Appuser);
        }
        [HttpGet]
        public IActionResult Edit(string Id) =>
             View(_context.AppUsers
             .FirstOrDefault(u => u.Id == Id));

        //[HttpPost]
        //public IActionResult EditUser(ApplicationUser model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Entry(model).State = EntityState.Modified;
        //        _context.SaveChanges();
        //        return RedirectToAction("list");
        //    }
        //    return View(model);
        //}

        public async Task<IActionResult> SendUsersEmail(string subject, string message)
        {
            try
            {
                var users = _context.AppUsers.ToList();

                foreach (var user in users)
                {
                    await _emailSender.SendEmailAsync(user.Email, subject, message);
                }
                return Json(new { success = true, responseText = "email was successful" });
            }
            catch (Exception)
            {
                return Json(new { success = true, responseText = "email was unsuccessful" });

            }
            finally
            {

            }
        }

            public async Task<IActionResult> AccreditUserCrypto(double CryptoReceived)
        {
            try
            {
                // var totalmined = _context.Entry("UserTotalMined");

                var batches = _context.MiningInventory.Where(p => p.Status == BatchStatus.Mining && DateTime.Compare(DateTime.Now, p.MiningStartDate.AddDays(1)) >= 0);
                // Mining Status && we are past the Mining Start Date

                var totalMhCapacity = 0;

                foreach (var batch in batches)
                {
                    totalMhCapacity += batch.MHAmount;

                    //  re-assigning of status if one day from reaching MiningEndDate
                    if (DateTime.Compare(DateTime.Now.AddDays(1), batch.MiningEndDate) >= 0)
                    {
                        batch.Status = BatchStatus.StoppedMining;
                    }
                }

                foreach (var batch in batches)
                {
                    var batch_id = batch.BatchId;
                    var rentalContracts = _context.RentalPurchaseContract.Where(p => p.Batch.BatchId == batch_id);

                    foreach (var contract in rentalContracts)
                    {
                        _context.Entry(contract).Reference(s => s.AppUser).Load();

                        var MhPurchased = contract.MHAmount;
                        var user = contract.AppUser;

                        var CryptoToAccredit = CryptoReceived * MhPurchased / totalMhCapacity;

                        // add new field in MiningContractProgress
                        var miningContractProgress = new MiningContractProgress
                        {
                            RentalPurchaseContract = contract,
                            AppUser = user,
                            CryptoMined = CryptoToAccredit,
                            Date = DateTime.Now
                        };
                        _context.Add(miningContractProgress);
                    }
                }
                await _context.SaveChangesAsync();

                return Json(new { success = true, responseText = "distribution was successful" });
            }
            catch (Exception e)
            {
                return Json(new { success = true, responseText = "distribution failed." });

                throw e;
            }

        }

    }
}