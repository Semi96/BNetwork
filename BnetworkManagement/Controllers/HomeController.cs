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
using HtmlAgilityPack;

namespace BnetworkManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(
         
           ApplicationDbContext context)
        {    
            _context = context;
        }

        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }
        public IActionResult Agreement()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult MiningShop(MiningShopViewModel model)
        {
            var cap = _context.AvailableCapacity.Find(1);

            if (cap == null)
            {
                var newCap = new Capacity
                {
                    TotalMegaHashAvailable = 17500,
                    UnitPrice = 38
                };
                _context.Update(newCap);
                _context.SaveChangesAsync();
                cap = newCap;
            }

            model.TotalMegaHashAvailable = cap.TotalMegaHashAvailable;

            model.MegaHashPriceRate = cap.UnitPrice;
            model.MineableCurrencies = Currencies.Ethereum;
           // model.TotalMegaHashAvailable = capacity.TotalMegaHashAvailable;

            return View(model);
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
