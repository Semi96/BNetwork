using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using Coinbase;
using Coinbase.Wallet;
using Coinbase.Models.Transaction;
using Microsoft.AspNetCore.Authorization;

namespace BnetworkManagement.Controllers
{
    public class CryptoController : Controller
    {
        public IActionResult Index()
        {
  

            return View();
        }

        [HttpPost] 
        [Authorize]
        public IActionResult Main()
        {
            var client = new Client("", "");
            // var account = client.GetAccount("").Balance;
            var transactions = client.GetTransactions("");
            
            //var transaction = client.SendMoney("", new TransactionSendModel
            //{
            //    To = "",
            //    Amount = 0.002M,
            //    Currency = "ETH",
            //});
         

            // var balance = account.Balance;




            return View();
        }


    }
}