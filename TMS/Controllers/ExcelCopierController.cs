using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data;
using TMS.VMs;

namespace TMS.Controllers
{
    public class ExcelCopierController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly IHttpContextAccessor http;
        private string excelFilePath;
        private readonly DocType OSDt;
        private readonly Account StockAc;
        private readonly Account CapitalAc;

        public ExcelCopierController(TMSContext context, UserManager<User> usrMgr, 
            IHttpContextAccessor http, IWebHostEnvironment env)
        {
            string[] paths = { env.WebRootPath, "excel" };
            excelFilePath = Path.Combine(paths);
            this.context = context;
            this.usrMgr = usrMgr;
            this.http = http;
            OSDt = context.DocTypes.Where(a => a.Name == "OS").FirstOrDefault();
            StockAc = context.Accounts.Where(a => a.Name == "STOCK").FirstOrDefault();
            CapitalAc = context.Accounts.Where(a => a.Name == "CAPITAL").FirstOrDefault();
        }

        public IActionResult UploadData()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UploadData(ExcelCopierVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;

            if (vm.File == null)
            {
                json.Value = "Error! Please select an excel file";
                return json;
            }
            int percentage = 0;
            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
            string fileName = Path.Combine(excelFilePath, uniqueName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await vm.File.CopyToAsync(stream);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    try
                    {
                        int lineNumber = 0;
                        while (reader.Read())
                        {
                            using (IDbContextTransaction trans = context.Database.BeginTransaction())
                            {
                                LedgerDetail doc = context.LedgerDetails.Where(a => a.DocTypeId == 0).OrderByDescending(a => a.DocId).FirstOrDefault();
                                if (reader.GetValue(0) != null && reader.GetValue(1) != null && reader.GetValue(2) != null)
                                {
                                    percentage = reader.RowCount / 100;
                                    lineNumber += 1;
                                    var category = reader.GetValue(0);
                                    var brand = reader.GetValue(1);
                                    var name = reader.GetValue(2);
                                    var lepCode = reader.GetValue(3);
                                    var oem = reader.GetValue(4);
                                    var guardCode = reader.GetValue(5);
                                    var forModels = reader.GetValue(6);
                                    var cost = reader.GetValue(7);
                                    var price = reader.GetValue(8);
                                    var stock = reader.GetValue(9);
                                    if (string.IsNullOrEmpty(category.ToString()))
                                    {
                                        json.Value = "Error! Category is empty on line number " + lineNumber;
                                        return json;
                                    }
                                    var pc = context.ProductCategories.Where(a => a.Name == category.ToString()).FirstOrDefault();
                                    if (pc == null)
                                    {
                                        json.Value = "Error! Category not found on line number " + lineNumber;
                                        return json;
                                    }
                                    if (string.IsNullOrEmpty(brand.ToString()))
                                    {
                                        json.Value = "Error! Brand is empty on line number " + lineNumber;
                                        return json;
                                    }
                                    var pb = context.Brands.Where(a => a.Name == brand.ToString()).FirstOrDefault();
                                    if (pb == null)
                                    {
                                        json.Value = "Error! Brand not found on line number " + lineNumber;
                                        return json;
                                    }
                                    DateTime currDate = SysAcc.GetLocalDate();
                                    if (string.IsNullOrEmpty(name.ToString()))
                                    {
                                        json.Value = "Error! Name is empty on line number " + lineNumber;
                                        return json;
                                    }
                                    ProductVM p = new ProductVM();
                                    p.ProductCategoryId = pc.Id;
                                    p.BrandId = pb.Id;
                                    p.Name = name.ToString();
                                    if (double.TryParse(cost.ToString(), out double cp))
                                    {
                                        p.CostPrice = cp;
                                    }
                                    if (double.TryParse(price.ToString(), out double salePrice))
                                    {
                                        p.SalePrice = salePrice;
                                    }
                                    if (double.TryParse(stock.ToString(), out double opStock))
                                    {
                                        p.OpeningStock = opStock;
                                    }
                                    var ap = AddProduct(p);
                                    if(ap.StatusCode != 200)
                                    {
                                        return ap;
                                    }
                                    trans.Commit();
                                    UpdateProgress(percentage);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        json.Value =  "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                        return json;
                    }
                }
            }
            System.IO.File.Delete(fileName);
            json.Value = "Data Copied successfully";
            json.StatusCode = 200;
            return json;
        }

        public IActionResult UploadEngines()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UploadEngines(ExcelCopierVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;

            if (vm.File == null)
            {
                json.Value = "Error! Please select an excel file";
                return json;
            }
            int percentage = 0;
            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
            string fileName = Path.Combine(excelFilePath, uniqueName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await vm.File.CopyToAsync(stream);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    try
                    {
                        int lineNumber = 0;
                        while (reader.Read())
                        {
                            using (IDbContextTransaction trans = context.Database.BeginTransaction())
                            {
                                if (reader.GetValue(0) != null && reader.GetValue(1) != null && reader.GetValue(2) != null)
                                {
                                    percentage = reader.RowCount / 100;
                                    lineNumber += 1;
                                    var carBrand = reader.GetValue(0);
                                    var engineNumber = reader.GetValue(1);
                                    var engineCC = reader.GetValue(2);
                                    var oilCapacity = reader.GetValue(3);
                                    var company = reader.GetValue(4);
                                    var oilFilter = reader.GetValue(5);
                                    var airFilter = reader.GetValue(6);
                                    var fuelFilter = reader.GetValue(7);
                                    var cabinFilter = reader.GetValue(8);
                                    if (string.IsNullOrEmpty(carBrand.ToString()))
                                    {
                                        json.Value = "Error! Car brand is empty on line number " + lineNumber;
                                        return json;
                                    }
                                    var pc = context.CarBrands.Where(a => (a.Name + " " + a.Model + " " + a.Year) == carBrand.ToString()).FirstOrDefault();
                                    if (pc == null)
                                    {
                                        json.Value = "Error! Car brand name not found on line number " + lineNumber;
                                        return json;
                                    }
                                    if (string.IsNullOrEmpty(engineNumber.ToString()))
                                    {
                                        json.Value = "Error! Engine number is empty on line number " + lineNumber;
                                        return json;
                                    }                                    
                                    if (string.IsNullOrEmpty(engineCC.ToString()))
                                    {
                                        json.Value = "Error! Engine CC is empty on line number " + lineNumber;
                                        return json;
                                    }
                                    CarEngine p = new CarEngine();
                                    p.CarBrandId = pc.Id;
                                    p.EngineNumber = engineNumber.ToString();
                                    p.EngineCC = engineCC.ToString();
                                    p.OilCapacity = oilCapacity.ToString();
                                    p.Company = company.ToString();
                                    p.OilFilter = oilFilter.ToString();
                                    p.AirFilter = airFilter.ToString();
                                    p.FuelFilter = fuelFilter.ToString();
                                    p.CabinFilter = cabinFilter.ToString();

                                    context.CarEngines.Add(p);
                                    context.SaveChanges();

                                    trans.Commit();
                                    UpdateProgress(percentage);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                        return json;
                    }
                }
            }
            System.IO.File.Delete(fileName);
            json.Value = "Data Copied successfully";
            json.StatusCode = 200;
            return json;
        }

        public void UpdateProgress(int percentage)
        {
            string key = "Progress";
            string value = percentage.ToString();
            CookieOptions op = new CookieOptions();
            op.Expires = DateTime.Now.AddHours(1);
            http.HttpContext.Response.Cookies.Append(key, value, op);
        }

        public JsonResult AddProduct(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                Product p = new Product();
                p.ProductCategoryId = vm.ProductCategoryId;
                p.BrandId = vm.BrandId;
                p.Name = vm.Name;
                p.CostPrice = vm.CostPrice;
                p.SalePrice = vm.SalePrice;
                p.IsActive = true;

                context.Products.Add(p);
                context.SaveChanges();

                vm.Id = p.Id;
                vm.TransactionDate = new DateTime(1980, 01, 01, 00, 00, 00);
                vm.TradeFirmId = 1;
                vm.TransactionStatusId = 1;
                vm.UserId = usrMgr.GetUserId(User);

                if(vm.OpeningStock > 0)
                {
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
                }

                json.StatusCode = 200;
                return json;
            }
            catch(Exception ex)
            {
                json.Value = "Error!  " + ex.Message + "" + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }
        public JsonResult Ledger(ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
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
    }
}
