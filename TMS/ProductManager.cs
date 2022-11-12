using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Data;
using TMS.VMs;

namespace TMS
{
    public static class ProductManager
    {
        public static JsonResult Add(TMSContext context, ProductVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    Product data = new Product();
                    data.Name = vm.Name;
                    data.ProductCategoryId = vm.ProductCategoryId;
                    data.Name = vm.Name;
                    data.CostPrice = vm.CostPrice;
                    data.SalePrice = vm.SalePrice;
                    data.IsActive = true;

                    context.Products.Add(data);
                    context.SaveChanges();

                    trans.Commit();
                    json.Value = data.Id;
                    json.StatusCode = 200;
                    return json;
                }
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
    }
}
