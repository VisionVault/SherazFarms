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
    public class ProductController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly DocType OSDt;
        private readonly Account StockAc;
        private readonly Account CapitalAc;

        public ProductController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            OSDt = context.DocTypes.Where(a => a.Name == "OS").FirstOrDefault();
            StockAc = context.Accounts.Where(a => a.Name == "STOCK").FirstOrDefault();
            CapitalAc = context.Accounts.Where(a => a.Name == "CAPITAL").FirstOrDefault();
        }

        public JsonResult Search(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.Products.Where(a => a.IsActive == true)
                    .Select(a => new
                    {
                        a.Id,
                        a.ProductCategoryId,
                        Category = a.Category.Name,
                        a.Name,
                        a.CostPrice,
                        a.SalePrice
                    }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    code = context.Products.Where(a => a.IsActive == true && (a.Name).ToLower().Contains(param.ToLower()))
                    .Select(a => new
                    {
                        a.Id,
                        a.ProductCategoryId,
                        Category = a.Category.Name,
                        a.Name,
                        a.CostPrice,
                        a.SalePrice
                    }).ToList();
                }
                json.Value = code;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
        public JsonResult GetAll()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.Products
                    .Select(a => new
                    {
                        a.Id,
                        a.ProductCategoryId,
                        Category = a.Category.Name,
                        a.BrandId,
                        Brand = a.Brand.Name,
                        a.Name,
                        a.CostPrice,
                        a.SalePrice,
                        a.IsActive,
                    }).OrderBy(a => a.Name).ToList();
                json.Value = code;
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
        public JsonResult GetById(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            long accId = !long.TryParse(vm["AccountId"].ToString(), out _) ? 0 : Convert.ToInt64(vm["AccountId"]);
            long proId = !long.TryParse(vm["ProductId"].ToString(), out _) ? 0 : Convert.ToInt64(vm["ProductId"]);
            try
            {
                var data = (from a in context.Products.Where(a => a.Id == proId && a.IsActive == true)
                            let lastCost = context.ProductLines.Where(b => b.TransactionStatusId != 3 && b.TradeFirmId == 1 && b.ProductId == proId
                                 && b.AccountId == accId && b.DocType.Name == "PV").OrderByDescending(b => b.TransactionDate).FirstOrDefault()
                            let lastRate = context.ProductLines.Where(b => b.TransactionStatusId != 3 && b.TradeFirmId == 1 && b.ProductId == proId
                                 && b.AccountId == accId && b.DocType.Name == "SV").OrderByDescending(b => b.TransactionDate).FirstOrDefault()
                            select new
                            {
                                a.Id,
                                a.Name,
                                Stock = context.ProductLines.Where(b => b.TransactionStatusId != 3 && b.TradeFirmId == 1 && b.ProductId == proId).Sum(b => b.Stock),
                                Cost = lastCost == null ? a.CostPrice : lastCost.Rate,
                                Rate = lastRate == null ? a.SalePrice : lastRate.Rate
                            }).FirstOrDefault();
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
        public JsonResult GetByName(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = (from a in context.Products.Where(a => a.Name == param && a.IsActive == true)

                            select new
                            {
                                a.Id,
                                a.ProductCategoryId,
                                Category = a.Category.Name,
                                a.Name,
                            }).FirstOrDefault();
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
            try
            {
                var code = (from a in context.Products.Where(a => a.Id == id)

                            select new
                            {
                                a.Id,
                                a.ProductCategoryId,
                                Category = a.Category.Name,
                                a.BrandId,
                                Brand = a.Brand.Name,
                                a.Name,
                                a.CostPrice,
                                a.SalePrice,
                                a.IsActive,
                                OpeningStock = context.ProductLines.Where(b => b.DocTypeId == OSDt.Id && b.ProductId == a.Id && b.TransactionStatusId != 3 && b.TradeFirmId == 1).FirstOrDefault().Stock,
                                OpeningValue = context.ProductLines.Where(b => b.DocTypeId == OSDt.Id && b.ProductId == a.Id && b.TransactionStatusId != 3 && b.TradeFirmId == 1).FirstOrDefault().StockValue,
                            }).FirstOrDefault();
                json.Value = code;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        //Reports

        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult Stock(IFormCollection vm)
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

                int catId = !int.TryParse(vm["ProductCategoryId"].ToString(), out _) ? 0 : Convert.ToInt32(vm["ProductCategoryId"]);
                long productId = !long.TryParse(vm["ProductId"].ToString(), out _) ? 0 : Convert.ToInt64(vm["ProductId"]);
                Product pro = context.Products.Where(a => a.Id == productId).FirstOrDefault();
                ViewBag.Product = "";
                if (pro != null)
                {
                    ViewBag.Product = pro.Name;
                }

                List<RPTStockVM> data = new List<RPTStockVM>();
                List<RPTStockVM> products = (from p in context.Products.Where(a => a.IsActive == true)

                                             select new RPTStockVM
                                             {
                                                 CategoryId = p.ProductCategoryId,
                                                 Category = p.Category.Name,
                                                 ProductId = p.Id,
                                                 Product = p.Name
                                             }).ToList();
                List<RPTStockVM> opening = (from l in context.ProductLines.Where(a => (a.TransactionDate < fromDate || a.DocTypeId == OSDt.Id)
                                            && a.TransactionStatusId != 3)
                                            group l.Stock by new { l.ProductId } into g
                                            select new RPTStockVM
                                            {
                                                ProductId = g.Key.ProductId,
                                                Opening = g.Sum(),
                                            }).ToList();
                List<RPTStockVM> INQty = (from l in context.ProductLines.Where(a => a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.Stock > 0
                                          && a.DocTypeId != OSDt.Id && a.TransactionStatusId != 3)
                                          group l.Qty by new { l.ProductId } into g
                                          select new RPTStockVM
                                          {
                                              ProductId = g.Key.ProductId,
                                              INQty = g.Sum(),
                                          }).ToList();
                List<RPTStockVM> OUTQty = (from l in context.ProductLines.Where(a => a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.Stock < 0
                                          && a.DocTypeId != OSDt.Id && a.TransactionStatusId != 3)
                                           group l.Qty by new { l.ProductId } into g
                                           select new RPTStockVM
                                           {
                                               ProductId = g.Key.ProductId,
                                               OUTQty = g.Sum(),
                                           }).ToList();
                List<RPTStockVM> closing = (from l in context.ProductLines.Where(a => a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                            group l.Stock by new { l.ProductId } into g
                                            select new RPTStockVM
                                            {
                                                ProductId = g.Key.ProductId,
                                                Closing = g.Sum(),
                                            }).ToList();
                data = (from p in products

                        join os in opening
                        on p.ProductId equals os.ProductId into oslj
                        from os in oslj.DefaultIfEmpty()

                        join inQty in INQty
                        on p.ProductId equals inQty.ProductId into inQtylj
                        from inQty in inQtylj.DefaultIfEmpty()

                        join outQty in OUTQty
                        on p.ProductId equals outQty.ProductId into outQtylj
                        from outQty in outQtylj.DefaultIfEmpty()

                        join cs in closing
                        on p.ProductId equals cs.ProductId into cslj
                        from cs in cslj.DefaultIfEmpty()

                        let cost = SysAcc.GetCost(context, p.ProductId)

                        select new RPTStockVM
                        {
                            CategoryId = p.CategoryId,
                            Category = p.Category,
                            ProductId = p.ProductId,
                            Product = p.Product,
                            Opening = os == null ? 0 : os.Opening,
                            INQty = inQty == null ? 0 : inQty.INQty,
                            OUTQty = outQty == null ? 0 : outQty.OUTQty,
                            Closing = cs == null ? 0 : cs.Closing,
                            Cost = cost,
                            Value = cs == null ? 0 : Math.Round(cs.Closing * cost, 0) 
                        }).ToList();
                data = data.OrderBy(a => a.Category).ThenBy(a => a.Product).ToList();
                if (catId > 0)
                {
                    data = data.Where(a => a.CategoryId == catId).ToList();
                }
                if (productId > 0)
                {
                    data = data.Where(a => a.ProductId == productId).ToList();
                }
                if(bool.TryParse(vm["Zero"].ToString(), out bool zero) && zero == false)
                {
                    data = data.Where(a => a.Opening + a.INQty + a.OUTQty + a.Closing > 0).ToList();
                }
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }

        }

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Add(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.ProductCategoryId == 0)
            {
                json.Value = "Error! Select category";
                return json;
            }
            if (vm.BrandId == 0)
            {
                json.Value = "Error! Select brand";
                return json;
            }
            if (string.IsNullOrEmpty(vm.Name))
            {
                json.Value = "Error! Name cannot be empty";
                return json;
            }
            if (OSDt == null)
            {
                json.Value = "Error! Opening stock Doc Type not found";
                return json;
            }
            if (StockAc == null)
            {
                json.Value = "Error! Stock Account not found";
                return json;
            }
            if (CapitalAc == null)
            {
                json.Value = "Error! Capital Account not found";
                return json;
            }
            var exists = Exists(vm.Name);
            if (exists.StatusCode != 200)
            {
                return exists;
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                    Product data = new Product();
                    data.ProductCategoryId = vm.ProductCategoryId;
                    data.BrandId = vm.BrandId;
                    data.Name = vm.Name;
                    data.CostPrice = vm.CostPrice;
                    data.SalePrice = vm.SalePrice;
                    data.IsActive = true;

                    context.Products.Add(data);
                    context.SaveChanges();

                    vm.Id = data.Id;
                    vm.TransactionDate = new DateTime(1980, 01, 01, 00, 00, 00);
                    vm.TradeFirmId = tf.Id;
                    vm.TransactionStatusId = 1;
                    vm.UserId = usrMgr.GetUserId(User);

                    var ledger = Ledger(vm);
                    if (ledger.StatusCode != 200)
                    {
                        return ledger;
                    }

                    var stock = Stock(vm);
                    if (stock.StatusCode != 200)
                    {
                        return stock;
                    }

                    trans.Commit();
                    json.Value = "Success";
                    json.StatusCode = 200;
                    return json;
                }
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }

        public IActionResult Edit(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Edit(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Id == 0)
            {
                json.Value = "Error! Invalid product id";
                return json;
            }

            if (vm.ProductCategoryId == 0)
            {
                json.Value = "Error! Select category";
                return json;
            }
            if (vm.BrandId == 0)
            {
                json.Value = "Error! Select brand";
                return json;
            }
            if (OSDt == null)
            {
                json.Value = "Error! Opening stock Doc Type not found";
                return json;
            }
            if (StockAc == null)
            {
                json.Value = "Error! Stock Account not found";
                return json;
            }
            if (CapitalAc == null)
            {
                json.Value = "Error! Capital Account not found";
                return json;
            }
            if (vm.InitialName != vm.Name)
            {
                var exists = Exists(vm.Name);
                if (exists.StatusCode != 200)
                {
                    return exists;
                }
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                    Product data = context.Products.Where(a => a.Id == vm.Id).FirstOrDefault();
                    data.ProductCategoryId = vm.ProductCategoryId;
                    data.BrandId = vm.BrandId;
                    data.Name = vm.Name;
                    data.CostPrice = vm.CostPrice;
                    data.SalePrice = vm.SalePrice;

                    context.SaveChanges();

                    vm.TransactionDate = new DateTime(1980, 01, 01, 00, 00, 00);
                    vm.TradeFirmId = tf.Id;
                    vm.TransactionStatusId = 2;
                    vm.UserId = usrMgr.GetUserId(User);

                    var remove = Remove(vm);
                    if (remove.StatusCode != 200)
                    {
                        return remove;
                    }

                    var ledger = Ledger(vm);
                    if (ledger.StatusCode != 200)
                    {
                        return ledger;
                    }

                    var stock = Stock(vm);
                    if (stock.StatusCode != 200)
                    {
                        return stock;
                    }

                    trans.Commit();
                    json.Value = "Success";
                    json.StatusCode = 200;
                    return json;
                }
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }

        public JsonResult Exists(string name)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.Products.Where(a => a.Name == name).FirstOrDefault();
                if (data == null)
                {
                    json.Value = "Success";
                    json.StatusCode = 200;
                    return json;
                }

                json.Value = "Error! Product already exists";
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public JsonResult Ledger(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                double amount = Math.Round(vm.OpeningStock * vm.CostPrice, 2);
                for (int i = 0; i < 2; i++)
                {
                    LedgerDetail l = new LedgerDetail();
                    l.DocId = 0;
                    l.DocTypeId = OSDt.Id;
                    l.TransactionDate = vm.TransactionDate;
                    l.PostDate = DateTime.Now;
                    l.ProductId = vm.Id;
                    if (i == 0)
                    {
                        l.AccountId = StockAc.Id;
                        l.AgainstAccountId = CapitalAc.Id;
                        l.Narration = "Opening Stock of " + vm.Name;
                        l.NarrationDetailed = "Opening Stock of " + vm.Name;
                        l.Debit = amount;
                        l.Credit = 0;
                    }
                    if (i == 1)
                    {
                        l.AccountId = CapitalAc.Id;
                        l.AgainstAccountId = StockAc.Id;
                        l.Narration = "Opening Stock of " + vm.Name;
                        l.NarrationDetailed = "Opening Stock of " + vm.Name;
                        l.Debit = 0;
                        l.Credit = amount;
                    }
                    l.TradeFirmId = vm.TradeFirmId;
                    l.TransactionStatusId = vm.TransactionStatusId;
                    l.UserId = vm.UserId;

                    context.LedgerDetails.Add(l);
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

        public JsonResult Stock(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                double amount = Math.Round(vm.OpeningStock * vm.CostPrice, 2);
                ProductLine pl = new ProductLine();
                pl.DocId = 0;
                pl.DocTypeId = OSDt.Id;
                pl.TransactionDate = vm.TransactionDate;
                pl.PostDate = DateTime.Now;
                pl.ProductId = vm.Id;
                pl.Stock = vm.OpeningStock;
                pl.StockValue = amount;
                pl.Qty = vm.OpeningStock;
                pl.Cost = vm.CostPrice;
                pl.Rate = vm.CostPrice;
                pl.Amount = amount;
                pl.Net = amount;
                pl.TradeFirmId = vm.TradeFirmId;
                pl.TransactionStatusId = vm.TransactionStatusId;
                pl.UserId = vm.UserId;

                context.ProductLines.Add(pl);
                context.SaveChanges();

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

        public JsonResult Remove(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                List<LedgerDetail> ld = context.LedgerDetails.Where(a => a.DocTypeId == OSDt.Id && a.ProductId == vm.Id).ToList();
                if (ld.Count > 0)
                {
                    context.LedgerDetails.RemoveRange(ld);
                    context.SaveChanges();
                }

                List<ProductLine> pl = context.ProductLines.Where(a => a.DocTypeId == OSDt.Id && a.ProductId == vm.Id).ToList();
                if (ld.Count > 0)
                {
                    context.ProductLines.RemoveRange(pl);
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
