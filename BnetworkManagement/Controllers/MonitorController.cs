using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BnetworkManagement.Models;
using BnetworkManagement.Models.AccountViewModels;
using BnetworkManagement.Models.BusinessViewModels;
using BnetworkManagement.Data;
using System.Text;
using System.Security.Cryptography;
using System.Net;

using System.IO;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BnetworkManagement.Controllers
{
    public class MonitorController : Controller

    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MonitorController(
         UserManager<ApplicationUser> userManager,
           ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> HiveOs()
        {
            try
            {
                string URL = "https://6cd5661f.ngrok.io/api/HiveOsStats/GetAverageStats";
                System.Net.WebRequest webRequest = System.Net.WebRequest.Create(URL);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                // Stream reqStream = webRequest.GetRequestStream();
                //  string postData = ""; //you form data in get format 
                //   byte[] postArray = Encoding.ASCII.GetBytes(postData);
                //reqStream.Write(postArray, 0, postArray.Length);
                //     reqStream.Close();
              //   var x = await webRequest.GetResponseAsync();
                StreamReader sr =  new StreamReader( webRequest.GetResponse().GetResponseStream());
                string Result =  sr.ReadToEnd();
                return Json(Result);
            }
            catch (Exception e)
            {
               
                var dataData = new { averageGpuTemp = 69.20, averageGpuFanSpeed = 70.02 };
                return Json(dataData);
            }
        }
       
        [HttpPost]
        public async Task<JsonResult> GetAccreditationsAsync()
        {
            var user = await _userManager.GetUserAsync(User);       
            // var userId = user.Id;
            var contextData = _context.MiningContractProgress.Where(x => x.AppUser == user).Select(a => new {date =  a.Date.ToString("ddd, dd MMM yyyy HH':'mm':'ss "), CryptoMined = Math.Round(a.CryptoMined , 8), currency = a.RentalPurchaseContract.Currency.ToString(), dateShort = a.Date.ToShortDateString() }).ToList();
            var dataData = new { data = contextData };
            var json = JsonConvert.SerializeObject(dataData);

            return Json(dataData);

        }

        [HttpPost]
        public JsonResult GetMessages()
        {
            var contextData = _context.Messages.Where(x => x.Status == MessageStatus.Posted).Select(a => new { a.MessageString }).ToList();
            var dataData = new { data = contextData };
            var json = JsonConvert.SerializeObject(dataData);

            return Json(dataData);

        }

        public async Task<JsonResult> GetUserWalletAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            _context.Entry(user).Reference(s => s.Wallet).Load();
            var walletKey = user.Wallet.WalletKey;

            var returnJson = new { data = walletKey };
            var json = JsonConvert.SerializeObject(returnJson);

            return Json(returnJson);
        }

        [HttpPost]
        public async Task<JsonResult> GetWithdrawsAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // var userId = user.Id;

            var contextData = _context.UserTransactions.Where(x => x.AppUser == user).Select(a => new { date = a.Date.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"), cryptoDelivered = Math.Round(a.CryptoRequested, 8), currency = "Ethereum" }).ToList();





            var dataData = new { data = contextData };
            var json = JsonConvert.SerializeObject(dataData);

            return Json(dataData);

        }

        [HttpPost]
        public async Task<JsonResult> GetTransactionsAsync()
        {
            var user =  await _userManager.GetUserAsync(User);
            string purchaseDate;
            string miningStatus;
            string currency;
            // var userId = user.Id;

            var contextData = _context.RentalPurchaseContract.Where(x => x.AppUser == user).Select( a => new { a.MHAmount, purchaseDate = a.PurchaseDate.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"), currency = a.Currency.ToString(), miningStatus = a.Batch.Status.ToString() }).ToList();
      




            var dataData = new { data = contextData };
               var json = JsonConvert.SerializeObject(dataData);
          
            return Json(dataData);
          
        }
        [Authorize]
        public IActionResult MyRig()
        {
            return View();
        }

       

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
