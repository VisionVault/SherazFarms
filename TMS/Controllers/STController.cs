using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Data;
using TMS.VMs;

namespace TMS.Controllers
{
    [Authorize]
    public class STController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly IHttpContextAccessor http;
        private readonly IWebHostEnvironment env;
        private readonly DocType STDT;
        private readonly Account CapitalAc;
        private readonly Account StockAc;
        private readonly Account CIHAc;

        public STController(TMSContext context, UserManager<User> usrMgr,
            IHttpContextAccessor http, IWebHostEnvironment env)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            this.http = http;
            this.env = env;
            STDT = context.DocTypes.Where(a => a.Name == "ST").FirstOrDefault();
            CapitalAc = context.Accounts.Where(a => a.Name == "Capital").FirstOrDefault();
            StockAc = context.Accounts.Where(a => a.Name == "Stock").FirstOrDefault();
            CIHAc = context.Accounts.Where(a => a.Name == "Cash in Hand").FirstOrDefault();
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
            if (STDT == null)
            {
                json.Value = "Error! Purchase Doc Type not found";
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
                var data = (from pl in context.ProductLines.Where(a => a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.Stock < 0
                && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3)
                            group pl.Qty by new { pl.DocId, pl.TransactionDate } into g
                            select new
                            {
                                g.Key.DocId,
                                g.Key.TransactionDate,
                                Date = g.Key.TransactionDate.ToString("dd/MM/yyyy"),
                                OutQty = g.Sum(),
                                InQty = context.ProductLines.Where(a => a.DocId == g.Key.DocId && a.Stock > 0 && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3).Sum(a => a.Qty)
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
            if (STDT == null)
            {
                json.Value = "Error! Purchase Doc Type not found";
                return json;
            }
            try
            {
                var data = (from pl in context.ProductLines.Where(a => a.DocId == id && a.DocTypeId == STDT.Id && a.Stock < 0 && a.TransactionStatusId != 3)

                            let inQty = context.ProductLines.Where(a => a.Id == (pl.Id + 1) && a.Stock > 0 && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3).FirstOrDefault()

                            select new
                            {
                                pl.DocId,
                                pl.TransactionDate,
                                Date = pl.TransactionDate.ToString("yyyy-MM-dd"),
                                OutProductId = pl.ProductId,
                                OutProduct = pl.Product.Name,
                                OutQty = pl.Qty,
                                InProductId = inQty == null ? 0 : inQty.ProductId,
                                InProduct = inQty == null ? "" : inQty.Product.Name,
                                InQty = inQty == null ? 0 : inQty.Qty,
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
        public IActionResult PurchaseReport(IFormCollection vm)
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

                List<SaleReportVM> data = (from p in context.ProductLines.Where(a => a.DocTypeId == STDT.Id && a.TransactionStatusId != 3
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
            ViewBag.DocId = Convert.ToInt64(GetMaxDocId(STDT.Id).Value);
            ViewBag.Date = SysAcc.GetLocalDate().ToString("yyyy-MM-dd");
            return View();
        }
        [HttpPost]
        public JsonResult Add(List<STVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (STDT == null)
            {
                json.Value = "Error! Stock Transfer Doc Type not found";
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
            if (CIHAc == null)
            {
                json.Value = "Error! Cash in Hand account not found";
                return json;
            }
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            STVM doc = vm.FirstOrDefault();
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.OutProductId == 0)
                {
                    json.Value = "Error! Invalid out product on line " + line;
                    return json;
                }
                if (item.OutQty == 0)
                {
                    json.Value = "Error! Invalid out qty on line " + line;
                    return json;
                }
                if (item.InProductId == 0)
                {
                    json.Value = "Error! Invalid in product on line " + line;
                    return json;
                }
                if (item.InQty == 0)
                {
                    json.Value = "Error! Invalid in qty on line " + line;
                    return json;
                }
            }
            var maxCode = GetMaxDocId(STDT.Id);
            if (maxCode.StatusCode != 200)
            {
                return maxCode;
            }

            doc.DocId = Convert.ToInt64(maxCode.Value);
            doc.TransactionDate = SysAcc.GetCurrentTime(doc.TransactionDate);
            doc.DocTypeId = STDT.Id;
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
                var param = new
                {
                    d = Convert.ToInt64(maxCode.Value),
                    t = STDT.Id
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
        public JsonResult Edit(List<STVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (STDT == null)
            {
                json.Value = "Error! Stock Transfer Doc Type not found";
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
            if (CIHAc == null)
            {
                json.Value = "Error! Cash in Hand account not found";
                return json;
            }
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            STVM doc = vm.FirstOrDefault();
            if (doc.DocId == 0)
            {
                json.Value = "Error! Invalid voucher id";
                return json;
            }
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.OutProductId == 0)
                {
                    json.Value = "Error! Invalid out product on line " + line;
                    return json;
                }
                if (item.OutQty == 0)
                {
                    json.Value = "Error! Invalid out qty on line " + line;
                    return json;
                }
                if (item.InProductId == 0)
                {
                    json.Value = "Error! Invalid in product on line " + line;
                    return json;
                }
                if (item.InQty == 0)
                {
                    json.Value = "Error! Invalid in qty on line " + line;
                    return json;
                }
            }
            doc.TransactionDate = SysAcc.GetCurrentTime(doc.TransactionDate);
            doc.DocTypeId = STDT.Id;
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
                    t = STDT.Id
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
            if (STDT == null)
            {
                json.Value = "Error! Purchase Doc Type not found";
                return json;
            }
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == id && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.TransactionStatusId = 3;
                        context.SaveChanges();
                    }
                }

                var pl = context.ProductLines.Where(a => a.DocId == id && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3).ToList();
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
            if (STDT == null)
            {
                json.Value = "Error! Purchase Doc Type not found";
                return json;
            }
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == docId && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    context.LedgerDetails.RemoveRange(data);
                    context.SaveChanges();
                }

                var pd = context.ProductLines.Where(a => a.DocId == docId && a.DocTypeId == STDT.Id && a.TransactionStatusId != 3).ToList();
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

        public JsonResult Ledger(List<STVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                STVM doc = vm.FirstOrDefault();
                double inProductStock = 0, outProductStock = 0;
                string inNarrationDetailed = "", outNarrationDetailed = "";
                foreach (var item in vm)
                {
                    double inCost = SysAcc.GetCost(context, item.InProductId, doc.DocId, doc.DocTypeId);
                    double inValue = item.InQty * inCost;
                    var inpd = context.Products.Where(a => a.Id == item.InProductId)
                            .Select(a => new {
                                a.Name,
                            }).FirstOrDefault();
                    double outCost = SysAcc.GetCost(context, item.OutProductId, doc.DocId, doc.DocTypeId);
                    double outValue = item.OutQty * outCost;
                    var outpd = context.Products.Where(a => a.Id == item.OutProductId)
                            .Select(a => new {
                                a.Name,
                            }).FirstOrDefault();
                    if (inpd != null)
                    {
                        inNarrationDetailed += item.InQty + " * " + inpd.Name + " @ " + inCost + Environment.NewLine;
                    }
                    if (outpd != null)
                    {
                        outNarrationDetailed += item.OutQty + " * " + outpd.Name + " @ " + outCost + Environment.NewLine;
                    }
                    inProductStock += inValue;
                    outProductStock += outValue;
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
                        ld.Narration = outNarrationDetailed;
                        ld.NarrationDetailed = outNarrationDetailed;
                        ld.Debit = outProductStock;
                        ld.Credit = 0;
                    }
                    if (i == 1)
                    {
                        ld.AccountId = StockAc.Id;
                        ld.AgainstAccountId = CapitalAc.Id;
                        ld.Narration = outNarrationDetailed;
                        ld.NarrationDetailed = outNarrationDetailed;
                        ld.Debit = 0;
                        ld.Credit = outProductStock;
                    }
                    if (i == 2)
                    {
                        ld.AccountId = StockAc.Id;
                        ld.AgainstAccountId = CapitalAc.Id;
                        ld.Narration = inNarrationDetailed;
                        ld.NarrationDetailed = inNarrationDetailed;
                        ld.Debit = inProductStock;
                        ld.Credit = 0;
                    }
                    if (i == 3)
                    {
                        ld.AccountId = CapitalAc.Id;
                        ld.AgainstAccountId = StockAc.Id;
                        ld.Narration = inNarrationDetailed;
                        ld.NarrationDetailed = inNarrationDetailed;
                        ld.Debit = 0;
                        ld.Credit = inProductStock;
                    }
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
        public JsonResult Stock(List<STVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                STVM doc = vm.FirstOrDefault();
                double cost = 0;
                foreach (var item in vm)
                {
                    for(int i = 0; i < 2; i++)
                    {
                        cost = 0;
                        ProductLine pl = new ProductLine();
                        pl.DocId = doc.DocId;
                        pl.DocTypeId = doc.DocTypeId;
                        pl.TransactionDate = doc.TransactionDate;
                        pl.PostDate = DateTime.Now;
                        if(i == 0)
                        {
                            cost = SysAcc.GetCost(context, item.OutProductId, doc.DocId, doc.DocTypeId);
                            pl.ProductId = item.OutProductId;
                            pl.Stock = item.OutQty * -1;
                            pl.StockValue = Math.Round(item.OutQty * cost, 2) * -1;
                            pl.Qty = item.OutQty;
                            pl.Cost = cost;
                            pl.Rate = cost;
                            pl.Amount = Math.Round(item.OutQty * cost, 2);
                            pl.Net = Math.Round(item.OutQty * cost, 2);
                        }
                        if (i == 1)
                        {
                            cost = SysAcc.GetCost(context, item.InProductId, doc.DocId, doc.DocTypeId);
                            pl.ProductId = item.InProductId;
                            pl.Stock = item.InQty;
                            pl.StockValue = Math.Round(item.InQty * cost, 2);
                            pl.Qty = item.InQty;
                            pl.Cost = cost;
                            pl.Rate = cost;
                            pl.Amount = Math.Round(item.InQty * cost, 2);
                            pl.Net = Math.Round(item.InQty * cost, 2);
                        }
                        pl.Payment = 0;
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
