using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Data;
using TMS.VMs;

namespace TMS.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;

        public ReportsController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
        }

        public IActionResult Index(string type)
        {
            ViewBag.FromDate = new DateTime(SysAcc.GetLocalDate().Year, SysAcc.GetLocalDate().Month, 1, 00, 00, 00).ToString("yyyy-MM-dd");
            ViewBag.ToDate = SysAcc.GetLocalDate().ToString("yyyy-MM-dd");
            ViewBag.Type = type;
            if (type == "Stock")
            {
                ViewBag.C = "Product";
                ViewBag.A = "Stock";
                ViewBag.Params = new string[] { "FromDate", "ToDate", "ProductCategoryId", "ProductId", "Zero"};
            }
            if (type == "Ledger")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "Ledger";
                ViewBag.Params = new string[] { "FromDate", "ToDate", "AccountId" };
            }
            if (type == "Receiveables")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "Receiveables";
                ViewBag.Params = new string[] { "ToDate" };
            }
            if (type == "Payables")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "Payables";
                ViewBag.Params = new string[] { "ToDate" };
            }
            if (type == "CashBook")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "CashBook";
                ViewBag.Params = new string[] { "ToDate" };
            }
            if (type == "BankBook")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "BankBook";
                ViewBag.Params = new string[] { "ToDate" };
            }
            if (type == "Purchase")
            {
                ViewBag.C = "Purchase";
                ViewBag.A = "PurchaseReport";
                ViewBag.Params = new string[] { "FromDate", "ToDate", "AccountId", "ProductId" };
            }
            if (type == "PurchaseReturn")
            {
                ViewBag.C = "PurchaseReturn";
                ViewBag.A = "PurchaseRetReport";
                ViewBag.Params = new string[] { "FromDate", "ToDate", "AccountId", "ProductId" };
            }
            if (type == "Sale")
            {
                ViewBag.C = "Sale";
                ViewBag.A = "SaleReport";
                ViewBag.Params = new string[] { "FromDate", "ToDate", "AccountId", "ProductId" };
            }
            if (type == "SaleReturn")
            {
                ViewBag.C = "SaleReturn";
                ViewBag.A = "SaleReturnReport";
                ViewBag.Params = new string[] { "FromDate", "ToDate", "AccountId", "ProductId" };
            }
            if (type == "Trial")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "Trial";
                ViewBag.Params = new string[] { "FromDate", "ToDate" };
            }
            if (type == "PL")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "ProfitLoss";
                ViewBag.Params = new string[] { "FromDate", "ToDate" };
            }
            if (type == "Balance Sheet")
            {
                ViewBag.C = "Accounts";
                ViewBag.A = "BalanceSheet";
                ViewBag.Params = new string[] { "ToDate" };
            }
            return View();
        }
        public IActionResult Invoice(long d, int t, bool hr)
        {
            if (d == 0 || t == 0)
            {
                return View();
            }
            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            ViewBag.TF = tf;
            List<RPTInvoiceVM> data = (from pl in context.ProductLines.Where(a => a.DocId == d
                                       && a.DocTypeId == t && a.TradeFirmId == tf.Id && a.TransactionStatusId != 3)

                                       let accDetail = context.AccountDetails.Where(b => b.AccountId == pl.AccountId)
                                       .FirstOrDefault()

                                       select new RPTInvoiceVM
                                       {
                                           HR = hr,
                                           DocId = pl.DocId,
                                           TransactionDate = pl.TransactionDate,
                                           Date = pl.TransactionDate.ToString("dd/MM/yy hh:mm tt"),
                                           Term = pl.PaymentTerm.Name,
                                           Account = pl.Account.Name,
                                           Contact = accDetail == null ? "" : accDetail.Contact,
                                           Address = accDetail == null ? "" : accDetail.Address,
                                           City = accDetail == null ? "" : accDetail.City.Name,
                                           VehicleNumber = pl.VehicleNumber,
                                           DriverName = pl.DriverName,
                                           Product = pl.Product.Name,
                                           Location = pl.Location.Name,
                                           EmptyWeight = pl.EmptyWeight,
                                           LoadedWeight = pl.LoadedWeight,
                                           ActualWeight = pl.ActualWeight,
                                           Rate = pl.Rate,
                                           Amount = pl.Amount,
                                           DiscountP = pl.DiscountP,
                                           Discount = pl.Discount,
                                           Net = pl.Net,
                                           BillDiscountP = pl.BillDiscountP,
                                           BillDiscount = pl.BillDiscount,
                                       }).ToList();
            if(data.Count == 0)
            {
                return View();
            }
            else
            {
                return View(data);
            }
        }
    }
}
