using TMS.Data;
using TMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMS.VMs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;

        public HomeController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
        }

        [HttpPost]
        public JsonResult Dashboard(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                DateTime currentDate = SysAcc.GetLocalDate();
                DateTime monthStart = new DateTime(currentDate.Year, currentDate.Month, 1, 00, 00, 00);
                DateTime monthEnd = currentDate;
                if (DateTime.TryParse(vm["FromDate"].ToString(), out DateTime sDate)
                    && DateTime.TryParse(vm["ToDate"].ToString(), out DateTime eDate))
                {
                    monthStart = sDate;
                    monthEnd = eDate;
                }

                DashboardVM data = new DashboardVM();

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                data.Sales = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.TransactionStatusId != 3 && a.Account.Name == "Sale"
            && a.TransactionDate >= monthStart && a.TransactionDate <= monthEnd).Sum(a => a.Credit - a.Debit);

                data.Purchases = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.TransactionStatusId != 3 && a.Account.Name == "Stock" && a.DocType.Name == "PV"
            && a.TransactionDate >= monthStart && a.TransactionDate <= monthEnd).Sum(a => a.Debit - a.Credit);

                var recPay = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && (a.Account.GAL.Name == "TRADE DEBTS" || a.Account.GAL.Name == "TRADE AND OTHER PAYABLES") && a.TransactionStatusId != 3)
                              group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                              select new RPTLedgerVM
                              {
                                  Account = g.Key.Account,
                                  Debit = g.Sum(a => a.Debit - a.Credit),
                              }).ToList();
                data.Receiveables = recPay.Where(a => a.Debit > 0).Sum(a => a.Debit);
                var payables = recPay.Where(a => a.Debit < 0).Sum(a => a.Debit);
                if (payables == 0)
                {
                    data.Payables = 0;
                }
                data.Payables = payables * -1;

                json.Value = data;
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }
        public IActionResult Index()
        {
            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            ViewBag.TF = "";
            if (tf != null)
            {
                ViewBag.TF = tf.Name;
            }
            return View();
        }
    }
}
