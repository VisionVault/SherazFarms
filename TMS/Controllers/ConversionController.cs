using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class ConversionController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly IHttpContextAccessor http;
        private readonly IWebHostEnvironment env;
        private readonly DocType ConversionDT;
        private readonly Account StockAc;
        private readonly Account CapitalAc;

        public ConversionController(TMSContext context, UserManager<User> usrMgr,
            IHttpContextAccessor http, IWebHostEnvironment env)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            this.http = http;
            this.env = env;
            ConversionDT = context.DocTypes.Where(a => a.Name == "CONVERSION").FirstOrDefault();
            StockAc = context.Accounts.Where(a => a.Name == "Stock").FirstOrDefault();
            CapitalAc = context.Accounts.Where(a => a.Name == "Capital").FirstOrDefault();
        }

        public JsonResult GetMaxDocId(int docTypeId)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                long docId = 0;
                var data = context.LedgerDetails.Where(a => a.DocTypeId == docTypeId).OrderByDescending(a => a.DocId).FirstOrDefault();

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
            if (ConversionDT == null)
            {
                json.Value = "Error! Conversion Doc Type not found";
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
                var data = (from pl in context.ProductLines.Where(a => a.TransactionDate >= fromDate && a.TransactionDate <= toDate
                && a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3 && a.Stock < 0)
                            let ready = context.ProductLines.Where(a => a.Id == pl.Id + 1).FirstOrDefault()
                            select new
                            {
                                pl.DocId,
                                pl.TransactionDate,
                                Date = pl.TransactionDate.ToString("dd/MM/yyyy"),
                                RawPro = pl.Product.Name,
                                RawQty = pl.Qty,
                                ReadyPro = ready.Product.Name,
                                ReadyQty = ready.Qty
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
            if (ConversionDT == null)
            {
                json.Value = "Error! Conversion Doc Type not found";
                return json;
            }
            try
            {
                var data = (from pl in context.ProductLines
                            .Where(a => a.DocId == id && a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3 && a.Stock < 0)
                            let ready = context.ProductLines.Where(a => a.Id == pl.Id + 1).FirstOrDefault()
                            select new
                            {
                                pl.DocId,
                                pl.TransactionDate,
                                Date = pl.TransactionDate.ToString("yyyy-MM-dd"),
                                RawProductId = pl.ProductId,
                                RawPro = pl.Product.Name,
                                RawQty = pl.Qty,
                                RawRate = pl.Rate,
                                RawAmount = pl.Amount,
                                ReadyProductId = ready.ProductId,
                                ReadyPro = ready.Product.Name,
                                ReadyQty = ready.Qty,
                                ReadyRate = ready.Rate,
                                ReadyAmount = ready.Amount
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
        public IActionResult SaleReport(IFormCollection vm)
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

                List<SaleReportVM> data = (from p in context.ProductLines.Where(a => a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3
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
                foreach(var item in data)
                {
                    if(docId != item.DocId)
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
            ViewBag.DocId = Convert.ToInt64(GetMaxDocId(ConversionDT.Id).Value);
            ViewBag.Date = SysAcc.GetLocalDate().ToString("yyyy-MM-dd");
            return View();
        }
        [HttpPost]
        public JsonResult Add(List<ConVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (ConversionDT == null)
            {
                json.Value = "Error! Conversion Doc Type not found";
                return json;
            }
            if (CapitalAc == null)
            {
                json.Value = "Error! Capital account not found";
                return json;
            }
            if (StockAc == null)
            {
                json.Value = "Error! Stock account not found";
                return json;
            }
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            ConVM doc = vm.FirstOrDefault();
            int line = 0;
            foreach(var item in vm)
            {
                line += 1;
                if(item.RawProductId == 0)
                {
                    json.Value = "Error! Invalid raw product on line " + line;
                    return json;
                }
                if (item.RawQty == 0)
                {
                    json.Value = "Error! Invalid raw qty on line " + line;
                    return json;
                }
                if (item.RawRate == 0)
                {
                    json.Value = "Error! Invalid raw rate on line " + line;
                    return json;
                }
                if (item.RawAmount == 0)
                {
                    json.Value = "Error! Invalid raw amount on line " + line;
                    return json;
                }
                if (item.ReadyProductId == 0)
                {
                    json.Value = "Error! Invalid ready product on line " + line;
                    return json;
                }
                if (item.ReadyQty == 0)
                {
                    json.Value = "Error! Invalid ready qty on line " + line;
                    return json;
                }
                if (item.ReadyRate == 0)
                {
                    json.Value = "Error! Invalid ready rate on line " + line;
                    return json;
                }
                if (item.ReadyAmount == 0)
                {
                    json.Value = "Error! Invalid ready amount on line " + line;
                    return json;
                }
            }
            var maxCode = GetMaxDocId(ConversionDT.Id);
            if (maxCode.StatusCode != 200)
            {
                return maxCode;
            }

            doc.DocId = Convert.ToInt64(maxCode.Value);
            doc.TransactionDate = SysAcc.GetCurrentTime(doc.TransactionDate);
            doc.DocTypeId = ConversionDT.Id;
            doc.TransactionStatusId = 1;
            doc.TFId = 1;
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
                var param = new { 
                    d = Convert.ToInt64(maxCode.Value),
                    t = ConversionDT.Id
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
        public JsonResult Edit(List<ConVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (ConversionDT == null)
            {
                json.Value = "Error! Conversion Doc Type not found";
                return json;
            }
            if (CapitalAc == null)
            {
                json.Value = "Error! Capital account not found";
                return json;
            }
            if (StockAc == null)
            {
                json.Value = "Error! Stock account not found";
                return json;
            }
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            ConVM doc = vm.FirstOrDefault();
            if (doc.DocId == 0)
            {
                json.Value = "Error! Invalid voucher id";
                return json;
            }
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.RawProductId == 0)
                {
                    json.Value = "Error! Invalid raw product on line " + line;
                    return json;
                }
                if (item.RawQty == 0)
                {
                    json.Value = "Error! Invalid raw qty on line " + line;
                    return json;
                }
                if (item.RawRate == 0)
                {
                    json.Value = "Error! Invalid raw rate on line " + line;
                    return json;
                }
                if (item.RawAmount == 0)
                {
                    json.Value = "Error! Invalid raw amount on line " + line;
                    return json;
                }
                if (item.ReadyProductId == 0)
                {
                    json.Value = "Error! Invalid ready product on line " + line;
                    return json;
                }
                if (item.ReadyQty == 0)
                {
                    json.Value = "Error! Invalid ready qty on line " + line;
                    return json;
                }
                if (item.ReadyRate == 0)
                {
                    json.Value = "Error! Invalid ready rate on line " + line;
                    return json;
                }
                if (item.ReadyAmount == 0)
                {
                    json.Value = "Error! Invalid ready amount on line " + line;
                    return json;
                }
            }

            doc.TransactionDate = SysAcc.GetCurrentTime(doc.TransactionDate);
            doc.DocTypeId = ConversionDT.Id;
            doc.TransactionStatusId = 2;
            doc.TFId = 1;
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
                    t = ConversionDT.Id
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
            if (ConversionDT == null)
            {
                json.Value = "Error! Conversion Doc Type not found";
                return json;
            }
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == id && a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.TransactionStatusId = 3;
                        context.SaveChanges();
                    }
                }

                var pl = context.ProductLines.Where(a => a.DocId == id && a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3).ToList();
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
            if (ConversionDT == null)
            {
                json.Value = "Error! Conversion Doc Type not found";
                return json;
            }
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == docId && a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    context.LedgerDetails.RemoveRange(data);
                    context.SaveChanges();
                }

                var pd = context.ProductLines.Where(a => a.DocId == docId && a.DocTypeId == ConversionDT.Id && a.TransactionStatusId != 3).ToList();
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

        public JsonResult Ledger(List<ConVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            string rawNarrationDetailed = "";
            string readyNarrationDetailed = "";
            try
            {
                ConVM doc = vm.FirstOrDefault();
                double rawAmount = vm.Sum(a => a.RawAmount), readyAmount = vm.Sum(a => a.ReadyAmount);
                foreach (var item in vm)
                {
                    var pd = context.Products.Where(a => a.Id == item.RawProductId)
                            .Select(a => new {
                                a.Name,
                            }).FirstOrDefault();
                    if (pd != null)
                    {
                        rawNarrationDetailed += item.RawQty + " * " + pd.Name + " @ " + item.RawRate + Environment.NewLine;
                    }
                }
                foreach (var item in vm)
                {
                    var pd = context.Products.Where(a => a.Id == item.ReadyProductId)
                            .Select(a => new {
                                a.Name,
                            }).FirstOrDefault();
                    if (pd != null)
                    {
                        readyNarrationDetailed += item.ReadyQty + " * " + pd.Name + " @ " + item.ReadyRate + Environment.NewLine;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    LedgerDetail ld = new LedgerDetail();
                    ld.DocId = doc.DocId;
                    ld.DocTypeId = doc.DocTypeId;
                    ld.TransactionDate = doc.TransactionDate;
                    ld.PostDate = SysAcc.GetLocalDate();
                    if (i == 0)
                    {
                        ld.AccountId = CapitalAc.Id;
                        ld.AgainstAccountId = StockAc.Id;
                        ld.Narration = rawNarrationDetailed;
                        ld.NarrationDetailed = rawNarrationDetailed;
                        ld.Debit = rawAmount;
                        ld.Credit = 0;
                    }
                    if (i == 1)
                    {
                        ld.AccountId = StockAc.Id;
                        ld.AgainstAccountId = CapitalAc.Id;
                        ld.Narration = rawNarrationDetailed;
                        ld.NarrationDetailed = rawNarrationDetailed;
                        ld.Debit = 0;
                        ld.Credit = rawAmount;
                    }
                    if (i == 2)
                    {
                        ld.AccountId = StockAc.Id;
                        ld.AgainstAccountId = CapitalAc.Id;
                        ld.Narration = readyNarrationDetailed;
                        ld.NarrationDetailed = readyNarrationDetailed;
                        ld.Debit = readyAmount;
                        ld.Credit = 0;
                    }
                    if (i == 3)
                    {
                        ld.AccountId = CapitalAc.Id;
                        ld.AgainstAccountId = StockAc.Id;
                        ld.Narration = readyNarrationDetailed;
                        ld.NarrationDetailed = readyNarrationDetailed;
                        ld.Debit = 0;
                        ld.Credit = readyAmount;
                    }
                    ld.PaymentTermId = null;
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
        public JsonResult Stock(List<ConVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                ConVM doc = vm.FirstOrDefault();
                foreach(var item in vm)
                {
                    for(int i = 0; i < 2; i++)
                    {
                        ProductLine pl = new ProductLine();
                        pl.DocId = doc.DocId;
                        pl.DocTypeId = doc.DocTypeId;
                        pl.TransactionDate = doc.TransactionDate;
                        pl.PostDate = DateTime.Now;
                        pl.AccountId = null;
                        if(i == 0)
                        {
                            pl.ProductId = item.RawProductId;
                            pl.Stock = item.RawQty * -1;
                            pl.StockValue = item.RawAmount * -1;
                            pl.Qty = item.RawQty;
                            pl.Cost = item.RawRate;
                            pl.Rate = item.RawRate;
                            pl.Amount = item.RawAmount;
                            pl.Net = item.RawAmount;
                        }
                        if (i == 1)
                        {
                            pl.ProductId = item.ReadyProductId;
                            pl.Stock = item.ReadyQty;
                            pl.StockValue = item.ReadyAmount;
                            pl.Qty = item.ReadyQty;
                            pl.Cost = item.ReadyRate;
                            pl.Rate = item.ReadyRate;
                            pl.Amount = item.ReadyAmount;
                            pl.Net = item.ReadyAmount;
                        }
                        pl.PaymentTermId = null;
                        pl.TradeFirmId = doc.TFId;
                        pl.TransactionStatusId = doc.TransactionStatusId;
                        pl.UserId = doc.UserId;

                        context.ProductLines.Add(pl);
                        context.SaveChanges();
                    }
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
