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
    public class RecipeController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly IHttpContextAccessor http;
        private readonly IWebHostEnvironment env;

        public RecipeController(TMSContext context, UserManager<User> usrMgr,
            IHttpContextAccessor http, IWebHostEnvironment env)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            this.http = http;
            this.env = env;
        }

        [HttpPost]
        public JsonResult GetAll(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = (from r in context.Recipes

                            select new
                            {
                                r.ProductId,
                                Product = r.Product.Name,
                            }).Distinct().ToList();
                data = data.OrderBy(a => a.Product).ToList();
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
                var data = (from r in context.Recipes.Where(a => a.ProductId == id)

                            select new
                            {
                                r.ProductId,
                                Product = r.Product.Name,
                                r.RawMaterialId,
                                RawMaterial = r.RawMaterial.Name,
                                r.Qty,
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

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Add(List<RecipeVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            RecipeVM doc = vm.FirstOrDefault();
            if (doc.ProductId == 0)
            {
                json.Value = "Error! Please select ready product";
                return json;
            }
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.RawMaterialId == 0)
                {
                    json.Value = "Error! Invalid raw material on line " + line;
                    return json;
                }
                if (item.Qty == 0)
                {
                    json.Value = "Error! Invalid qty on line " + line;
                    return json;
                }
            }

            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    foreach (var item in vm)
                    {
                        Recipe r = new Recipe();
                        r.ProductId = doc.ProductId;
                        r.RawMaterialId = item.RawMaterialId;
                        r.Qty = item.Qty;

                        context.Recipes.Add(r);
                        context.SaveChanges();
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
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Edit(List<RecipeVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            RecipeVM doc = vm.FirstOrDefault();
            if (doc.ProductId == 0)
            {
                json.Value = "Error! Please select ready product";
                return json;
            }
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.RawMaterialId == 0)
                {
                    json.Value = "Error! Invalid raw material on line " + line;
                    return json;
                }
                if (item.Qty == 0)
                {
                    json.Value = "Error! Invalid qty on line " + line;
                    return json;
                }
            }

            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    List<Recipe> oldR = context.Recipes.Where(a => a.ProductId == doc.ProductId).ToList();
                    if (oldR.Count > 0)
                    {
                        context.Recipes.RemoveRange(oldR);
                        context.SaveChanges();
                    }

                    foreach (var item in vm)
                    {
                        Recipe r = new Recipe();
                        r.ProductId = doc.ProductId;
                        r.RawMaterialId = item.RawMaterialId;
                        r.Qty = item.Qty;

                        context.Recipes.Add(r);
                        context.SaveChanges();
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

        public JsonResult Delete(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                List<Recipe> data = context.Recipes.Where(a => a.ProductId == id).ToList();
                if (data.Count > 0)
                {
                    context.Recipes.RemoveRange(data);
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
    }
}
