using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMS.Data;
using TMS.VMs;

namespace TMS.Controllers
{
    [Authorize]
    public class SaleReturnController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly IHttpContextAccessor http;
        private readonly IWebHostEnvironment env;
        private readonly GAL CusGAL;
        private readonly GAL CABGAL;
        private readonly DocType SaleRetDT;
        private readonly Account SaleRetAc;
        private readonly Account DiscAll;
        private readonly Account DeliveryAc;
        private readonly Account StockAc;
        private readonly Account CommAc;
        private readonly Account CGS;
        private readonly Account CIHAc;

        public SaleReturnController(TMSContext context, UserManager<User> usrMgr,
            IHttpContextAccessor http, IWebHostEnvironment env)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            this.http = http;
            this.env = env;
            CusGAL = context.GALs.Where(a => a.Name == "TRADE DEBTS").FirstOrDefault();
            CABGAL = context.GALs.Where(a => a.Name == "CASH AT BANK").FirstOrDefault();
            SaleRetDT = context.DocTypes.Where(a => a.Name == "SRV").FirstOrDefault();
            DiscAll = context.Accounts.Where(a => a.Name == "DISCOUNT ALLOWED").FirstOrDefault();
            DeliveryAc = context.Accounts.Where(a => a.Name == "DELIVERY").FirstOrDefault();
            SaleRetAc = context.Accounts.Where(a => a.Name == "Sale Return").FirstOrDefault();
            StockAc = context.Accounts.Where(a => a.Name == "Stock").FirstOrDefault();
            CommAc = context.Accounts.Where(a => a.Name == "COMMISSION").FirstOrDefault();
            CGS = context.Accounts.Where(a => a.Name == "CGS").FirstOrDefault();
            CIHAc = context.Accounts.Where(a => a.Name == "Cash in Hand").FirstOrDefault();
        }

        public JsonResult GetMaxDocId(int docTypeId)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                long docId = 0;
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocTypeId == docTypeId).OrderByDescending(a => a.DocId).FirstOrDefault();

                if (data != null)
                {
                    docId = data.DocId + 1;
                }
                else
                {
                    docId = 1;
                }

                json.Value = docId;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
        [HttpPost]
        public JsonResult GetAll(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (SaleRetDT == null)
            {
                json.Value = "Error! Sale Return Doc Type not found";
                return json;
            }
            if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
            {
                json.Value = "Error! Invalid from date";
                return json;
            }
            if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
            {
                json.Value = "Error! Invalid to date";
                return json;
            }
            DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
            DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = (from pl in context.ProductLines.Where(a => a.TradeFirmId == tf.Id && a.TransactionDate >= fromDate && a.TransactionDate <= toDate
                            && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3)
                            group new { pl.Qty, pl.Amount} by new 
                            { 
                                pl.DocId,
                                pl.TransactionDate, 
                                pl.VehicleNumber,
                                pl.DriverName,
                                Account = pl.Account.Name, PaymentTerm = pl.PaymentTerm.Name
                            } into g

                            select new
                            {
                                g.Key.DocId,
                                g.Key.TransactionDate,
                                Date = g.Key.TransactionDate.ToString("dd/MM/yyyy"),
                                g.Key.VehicleNumber,
                                g.Key.DriverName,
                                g.Key.Account,
                                g.Key.PaymentTerm,
                                Qty = g.Sum(a => a.Qty),
                                Amount = g.Sum(a => a.Amount),
                            }).ToList();
                data = data.OrderByDescending(a => a.TransactionDate).ThenByDescending(a => a.DocId).ToList();
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
        public JsonResult Recall(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (SaleRetDT == null)
            {
                json.Value = "Error! Sale Return Doc Type not found";
                return json;
            }
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = (from pl in context.ProductLines
                            .Where(a => a.TradeFirmId == tf.Id && a.DocId == id && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3)

                            let bankAc = context.LedgerDetails.Where(b => b.DocId == pl.DocId && b.DocTypeId == SaleRetDT.Id
                            && b.TransactionStatusId != 3 && b.Account.GALId == CABGAL.Id).FirstOrDefault()
                            let cusCar = context.CustomerCars.Where(b => b.AccountId == pl.AccountId).FirstOrDefault()
                            select new
                            {
                                pl.DocId,
                                pl.TransactionDate,
                                Date = pl.TransactionDate.ToString("yyyy-MM-dd"),
                                BankAcId = bankAc == null ? 0 : bankAc.AccountId,
                                BankAc = bankAc == null ? "" : bankAc.Account.Name,
                                pl.PaymentTermId,
                                PaymentTerm = pl.PaymentTerm.Name,
                                pl.AccountId,
                                Account = pl.Account.Name,
                                Contact = context.AccountDetails.Where(b => b.AccountId == pl.AccountId).FirstOrDefault().Contact,
                                pl.VehicleNumber,
                                pl.DriverName,
                                pl.ProductId,
                                Product = pl.Product.Name,
                                pl.Qty,
                                pl.Rate,
                                pl.Amount,
                                pl.DiscountP,
                                pl.Discount,
                                pl.Net,
                                pl.BillDiscountP,
                                pl.BillDiscount,
                                pl.Payment,
                            }).ToList();

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
            ViewBag.FromDate = new DateTime(SysAcc.GetLocalDate().Year, SysAcc.GetLocalDate().Month, 1, 00, 00, 00).ToString("yyyy-MM-dd");
            ViewBag.ToDate = SysAcc.GetLocalDate().ToString("yyyy-MM-dd");
            return View();
        }

        //reports
        [HttpPost]
        public IActionResult SaleReturnReport(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");

                long accountId = !long.TryParse(vm["AccountId"], out _) ? 0 : Convert.ToInt64(vm["AccountId"]);
                long pId = !long.TryParse(vm["ProductId"], out _) ? 0 : Convert.ToInt64(vm["ProductId"]);
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<SaleReportVM> data = (from p in context.ProductLines.Where(a => a.TradeFirmId == tf.Id && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3
                                          && a.TransactionDate >= fromDate && a.TransactionDate <= toDate)

                                           select new SaleReportVM
                                           {
                                               DocId = p.DocId,
                                               TransactionDate = p.TransactionDate,
                                               Date = p.TransactionDate.ToString("dd/MM/yyyy"),
                                               AccountId = p.AccountId,
                                               Account = p.Account.Name,
                                               ProductId = p.ProductId,
                                               Product = p.Product.Name,
                                               Qty = p.Qty,
                                               Rate = p.Rate,
                                               Amount = p.Amount,
                                               BillDiscount = p.BillDiscount,
                                           }).ToList();
                data = data.OrderBy(a => a.TransactionDate).ToList();
                data = data.OrderBy(a => a.DocId).ToList();
                if (accountId > 0)
                {
                    data = data.Where(a => a.AccountId == accountId).ToList();
                }
                if (pId > 0)
                {
                    data = data.Where(a => a.ProductId == pId).ToList();
                }
                long docId = 0;
                foreach (var item in data)
                {
                    if (docId != item.DocId)
                    {
                        item.IsGroupChanged = true;
                    }
                    docId = item.DocId;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message;
                return View();
            }

        }

        public IActionResult Add()
        {
            ViewBag.DocId = Convert.ToInt64(GetMaxDocId(SaleRetDT.Id).Value);
            ViewBag.Date = SysAcc.GetLocalDate().ToString("yyyy-MM-dd");
            return View();
        }
        [HttpPost]
        public JsonResult Add(List<DocVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (SaleRetDT == null)
            {
                json.Value = "Error! Sale Return Doc Type not found";
                return json;
            }
            if (SaleRetAc == null)
            {
                json.Value = "Error! Sale return account not found";
                return json;
            }
            if (DiscAll == null)
            {
                json.Value = "Error! Discount allowed account not found";
                return json;
            }
            if (DeliveryAc == null)
            {
                json.Value = "Error! Delivery account not found";
                return json;
            }
            if (CGS == null)
            {
                json.Value = "Error! CGS account not found";
                return json;
            }
            if (StockAc == null)
            {
                json.Value = "Error! Stock account not found";
                return json;
            }
            if (CIHAc == null)
            {
                json.Value = "Error! Cash in Hand account not found";
                return json;
            }
            if (CommAc == null)
            {
                json.Value = "Error! Commission account not found";
                return json;
            }
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            DocVM doc = vm.FirstOrDefault(); if (doc.AccountId == 0)
            {
                json.Value = "Error! Please select customer from list";
                return json;
            }
            if (string.IsNullOrEmpty(doc.VehicleNumber))
            {
                json.Value = "Error! Please enter vehicle number";
                return json;
            }
            if (string.IsNullOrEmpty(doc.DriverName))
            {
                json.Value = "Error! Please enter driver name";
                return json;
            }
            if (doc.AccountId == 0)
            {
                var customer = context.AccountDetails.Where(a => a.Contact == doc.Contact).FirstOrDefault();
                if (customer == null)
                {
                    if (string.IsNullOrEmpty(doc.Name))
                    {
                        json.Value = "Error! Please enter name";
                        return json;
                    }
                    AccountVM cus = new AccountVM();
                    cus.Name = doc.Name;
                    cus.Contact = doc.Contact;
                    cus.UserId = usrMgr.GetUserId(User);
                    var addcus = CustomerManager.AddCustomer(context, cus);
                    if (addcus.StatusCode != 200)
                    {
                        return addcus;
                    }
                    doc.AccountId = Convert.ToInt64(addcus.Value);
                }
                else
                {
                    doc.AccountId = customer.AccountId;
                }
            }
            if (doc.PaymentTermId == 3 && doc.BankAcId == 0)
            {
                json.Value = "Error! Please select a bank";
                return json;
            }
            int line = 0;
            foreach(var item in vm)
            {
                line += 1;
                if (item.ProductId == 0)
                {
                    json.Value = "Error! Invalid product on line " + line;
                    return json;
                }
                if (item.LocationId == 0)
                {
                    json.Value = "Error! Invalid location on line " + line;
                    return json;
                }
                if (item.ActualWeight == 0)
                {
                    json.Value = "Error! Invalid weight on line " + line;
                    return json;
                }
                if (item.Net == 0)
                {
                    json.Value = "Error! Invalid amount on line " + line;
                    return json;
                }
            }
            var maxCode = GetMaxDocId(SaleRetDT.Id);
            if (maxCode.StatusCode != 200)
            {
                return maxCode;
            }

            doc.DocId = Convert.ToInt64(maxCode.Value);
            doc.TransactionDate = SysAcc.GetCurrentTime(doc.TransactionDate);
            doc.DocTypeId = SaleRetDT.Id;
            doc.TransactionStatusId = 1;
            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            doc.TFId = tf.Id;
            doc.UserId = usrMgr.GetUserId(User);

            using (IDbContextTransaction trans = context.Database.BeginTransaction())
            {
                var led = Ledger(vm);
                if (led.StatusCode != 200)
                {
                    return led;
                }
                var stock = Stock(vm);
                if (stock.StatusCode != 200)
                {
                    return stock;
                }

                trans.Commit();
                var param = new
                {
                    d = Convert.ToInt64(maxCode.Value),
                    t = SaleRetDT.Id
                };
                json.Value = param;
                json.StatusCode = 200;
                return json;
            }
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Edit(List<DocVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (SaleRetDT == null)
            {
                json.Value = "Error! Sale Return Doc Type not found";
                return json;
            }
            if (SaleRetAc == null)
            {
                json.Value = "Error! Sale return account not found";
                return json;
            }
            if (DiscAll == null)
            {
                json.Value = "Error! Discount allowed account not found";
                return json;
            }
            if (DeliveryAc == null)
            {
                json.Value = "Error! Delivery account not found";
                return json;
            }
            if (CGS == null)
            {
                json.Value = "Error! CGS account not found";
                return json;
            }
            if (StockAc == null)
            {
                json.Value = "Error! Stock account not found";
                return json;
            }
            if (CIHAc == null)
            {
                json.Value = "Error! Cash in Hand account not found";
                return json;
            }
            if (CommAc == null)
            {
                json.Value = "Error! Commission account not found";
                return json;
            }
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            DocVM doc = vm.FirstOrDefault();
            if (doc.DocId == 0)
            {
                json.Value = "Error! Invalid voucher id";
                return json;
            }
            if (doc.AccountId == 0)
            {
                json.Value = "Error! Please select customer from list";
                return json;
            }
            if (string.IsNullOrEmpty(doc.VehicleNumber))
            {
                json.Value = "Error! Please enter vehicle number";
                return json;
            }
            if (string.IsNullOrEmpty(doc.DriverName))
            {
                json.Value = "Error! Please enter driver name";
                return json;
            }
            if (doc.AccountId == 0)
            {
                var customer = context.AccountDetails.Where(a => a.Contact == doc.Contact).FirstOrDefault();
                if (customer == null)
                {
                    if (string.IsNullOrEmpty(doc.Name))
                    {
                        json.Value = "Error! Please enter name";
                        return json;
                    }
                    AccountVM cus = new AccountVM();
                    cus.Name = doc.Name;
                    cus.Contact = doc.Contact;
                    cus.UserId = usrMgr.GetUserId(User);
                    var addcus = CustomerManager.AddCustomer(context, cus);
                    if (addcus.StatusCode != 200)
                    {
                        return addcus;
                    }
                    doc.AccountId = Convert.ToInt64(addcus.Value);
                }
                else
                {
                    doc.AccountId = customer.AccountId;
                }
            }
            if (doc.PaymentTermId == 3 && doc.BankAcId == 0)
            {
                json.Value = "Error! Please select a bank";
                return json;
            }
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.ProductId == 0)
                {
                    json.Value = "Error! Invalid product on line " + line;
                    return json;
                }
                if (item.LocationId == 0)
                {
                    json.Value = "Error! Invalid location on line " + line;
                    return json;
                }
                if (item.ActualWeight == 0)
                {
                    json.Value = "Error! Invalid weight on line " + line;
                    return json;
                }
                if (item.Net == 0)
                {
                    json.Value = "Error! Invalid amount on line " + line;
                    return json;
                }
            }

            doc.TransactionDate = SysAcc.GetCurrentTime(doc.TransactionDate);
            doc.DocTypeId = SaleRetDT.Id;
            doc.TransactionStatusId = 2;
            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            doc.TFId = tf.Id;
            doc.UserId = usrMgr.GetUserId(User);

            using (IDbContextTransaction trans = context.Database.BeginTransaction())
            {
                var delMsg = Delete(doc.DocId);
                if (delMsg.StatusCode != 200)
                {
                    return delMsg;
                }
                var led = Ledger(vm);
                if (led.StatusCode != 200)
                {
                    return led;
                }
                var stock = Stock(vm);
                if (stock.StatusCode != 200)
                {
                    return stock;
                }
                trans.Commit();
                var param = new
                {
                    d = doc.DocId,
                    t = SaleRetDT.Id
                };
                json.Value = param;
                json.StatusCode = 200;
                return json;
            }
        }

        public JsonResult Remove(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (SaleRetDT == null)
            {
                json.Value = "Error! Sale Return Doc Type not found";
                return json;
            }
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocId == id && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.TransactionStatusId = 3;
                        context.SaveChanges();
                    }
                }

                var pl = context.ProductLines.Where(a => a.TradeFirmId == tf.Id && a.DocId == id && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3).ToList();
                if (pl.Count > 0)
                {
                    foreach (var item in pl)
                    {
                        item.TransactionStatusId = 3;
                        context.SaveChanges();
                    }
                }
                json.Value = "Success";
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
        public JsonResult Delete(long docId)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (SaleRetDT == null)
            {
                json.Value = "Error! Sale Return Doc Type not found";
                return json;
            }
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocId == docId && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    context.LedgerDetails.RemoveRange(data);
                    context.SaveChanges();
                }

                var pd = context.ProductLines.Where(a => a.TradeFirmId == tf.Id && a.DocId == docId && a.DocTypeId == SaleRetDT.Id && a.TransactionStatusId != 3).ToList();
                if (pd.Count > 0)
                {
                    context.ProductLines.RemoveRange(pd);
                    context.SaveChanges();
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public JsonResult Ledger(List<DocVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            string narrationDetailed = "";
            try
            {
                DocVM doc = vm.FirstOrDefault();
                double amount = vm.Sum(a => a.Amount), discount = vm.Sum(a => a.Discount) + doc.BillDiscount, net = amount - discount, cost = 0;
                foreach (var item in vm)
                {
                    var pd = context.Products.Where(a => a.Id == item.ProductId)
                            .Select(a => new {
                                a.Name,
                            }).FirstOrDefault();
                    if (pd != null)
                    {
                        narrationDetailed += item.ActualWeight + "-Kgs " + pd.Name + " @ " + item.Rate + Environment.NewLine;
                    }
                    cost += Math.Round(item.ActualWeight * SysAcc.GetCost(context, item.ProductId, doc.DocId, doc.DocTypeId), 2);
                }
                narrationDetailed += "Vehicle.# " + doc.VehicleNumber + " --- " + " Driver " + doc.DriverName + Environment.NewLine;
                for (int i = 0; i < 7; i++)
                {
                    LedgerDetail ld = new LedgerDetail();
                    ld.DocId = doc.DocId;
                    ld.DocTypeId = doc.DocTypeId;
                    ld.TransactionDate = doc.TransactionDate;
                    ld.PostDate = SysAcc.GetLocalDate();
                    ld.DueDate = doc.DueDate;
                    if (i == 0)
                    {
                        ld.AccountId = SaleRetAc.Id;
                        ld.AgainstAccountId = doc.AccountId;
                        ld.Narration = narrationDetailed;
                        ld.NarrationDetailed = narrationDetailed;
                        ld.Debit = amount;
                        ld.Credit = 0;
                    }
                    if (i == 1)
                    {
                        if(discount == 0)
                        {
                            continue;
                        }
                        else
                        {
                            ld.AccountId = DiscAll.Id;
                            ld.AgainstAccountId = doc.AccountId;
                            ld.Narration = "Discount reversed on invoice " + doc.DocId;
                            ld.NarrationDetailed = "Discount reversed on invoice " + doc.DocId;
                            ld.Debit = 0;
                            ld.Credit = discount;
                        }
                    }
                    if (i == 2)
                    {
                        ld.AccountId = doc.AccountId;
                        ld.AgainstAccountId = SaleRetAc.Id;
                        ld.Narration = narrationDetailed;
                        ld.NarrationDetailed = narrationDetailed;
                        ld.Debit = 0;
                        ld.Credit = net;
                    }
                    if (i == 3)
                    {
                        ld.AccountId = StockAc.Id;
                        ld.AgainstAccountId = CGS.Id;
                        ld.Narration = narrationDetailed;
                        ld.NarrationDetailed = narrationDetailed;
                        ld.Debit = cost;
                        ld.Credit = 0;
                    }
                    if (i == 4)
                    {
                        ld.AccountId = CGS.Id;
                        ld.AgainstAccountId = StockAc.Id;
                        ld.Narration = narrationDetailed;
                        ld.NarrationDetailed = narrationDetailed;
                        ld.Debit = 0;
                        ld.Credit = cost;
                    }
                    if (i == 5)
                    {
                        if (doc.Payment == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (doc.PaymentTermId == 3)
                            {
                                ld.AccountId = doc.AccountId;
                                ld.AgainstAccountId = doc.BankAcId;
                                ld.Narration = "Cash paid through bank on invoice " + doc.DocId;
                                ld.NarrationDetailed = "Cash paid through bank on invoice " + doc.DocId;
                                ld.Debit = doc.Payment;
                                ld.Credit = 0;
                            }
                            else
                            {
                                ld.AccountId = doc.AccountId;
                                ld.AgainstAccountId = CIHAc.Id;
                                ld.Narration = "Cash paid on invoice " + doc.DocId;
                                ld.NarrationDetailed = "Cash paid on invoice " + doc.DocId;
                                ld.Debit = doc.Payment;
                                ld.Credit = 0;
                            }
                        }
                    }
                    if (i == 6)
                    {
                        if (doc.Payment == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if(doc.PaymentTermId == 3)
                            {
                                ld.AccountId = (long)doc.BankAcId;
                                ld.AgainstAccountId = doc.AccountId;
                                ld.Narration = "Cash paid through bank on invoice " + doc.DocId;
                                ld.NarrationDetailed = "Cash paid through bank on invoice " + doc.DocId;
                                ld.Debit = 0;
                                ld.Credit = doc.Payment;
                            }
                            else
                            {
                                ld.AccountId = CIHAc.Id;
                                ld.AgainstAccountId = doc.AccountId;
                                ld.Narration = "Cash paid on invoice " + doc.DocId;
                                ld.NarrationDetailed = "Cash paid on invoice " + doc.DocId;
                                ld.Debit = 0;
                                ld.Credit = doc.Payment;
                            }
                            
                        }
                    }
                    ld.PaymentTermId = doc.PaymentTermId;
                    ld.TradeFirmId = doc.TFId;
                    ld.TransactionStatusId = doc.TransactionStatusId;
                    ld.UserId = doc.UserId;

                    context.LedgerDetails.Add(ld);
                    context.SaveChanges();
                }

                json.Value = doc.DocId;
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }
        public JsonResult Stock(List<DocVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                DocVM doc = vm.FirstOrDefault();
                double cost = 0;
                foreach (var item in vm)
                {
                    ProductLine pl = new ProductLine();
                    pl.DocId = doc.DocId;
                    pl.DocTypeId = doc.DocTypeId;
                    pl.TransactionDate = doc.TransactionDate;
                    pl.PostDate = DateTime.Now;
                    pl.AccountId = doc.AccountId;
                    pl.VehicleNumber = doc.VehicleNumber;
                    pl.DriverName = doc.DriverName;
                    pl.Remarks = doc.Remarks;
                    pl.ProductId = item.ProductId;
                    pl.LocationId = item.LocationId;
                    pl.Stock = item.ActualWeight;
                    pl.StockValue = Math.Round(item.ActualWeight * cost, 2);
                    pl.EmptyWeight = item.EmptyWeight;
                    pl.LoadedWeight = item.LoadedWeight;
                    pl.ActualWeight = item.ActualWeight;
                    pl.Qty = item.ActualWeight;
                    pl.Cost = cost;
                    pl.Rate = item.Rate;
                    pl.Amount = item.Amount;
                    pl.DiscountP = item.DiscountP;
                    pl.Discount = item.Discount;
                    pl.Net = item.Amount - item.Discount;
                    pl.BillDiscountP = doc.BillDiscountP;
                    pl.BillDiscount = doc.BillDiscount;
                    pl.Payment = doc.Payment;
                    pl.PaymentTermId = doc.PaymentTermId;
                    pl.TradeFirmId = doc.TFId;
                    pl.TransactionStatusId = doc.TransactionStatusId;
                    pl.UserId = doc.UserId;

                    context.ProductLines.Add(pl);
                    context.SaveChanges();
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }
    }
}
