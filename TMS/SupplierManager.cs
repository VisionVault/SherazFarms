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
    public static class SupplierManager
    {
        public static JsonResult GetMaxCode(TMSContext context, int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                string code = "01";
                var data = context.Accounts.Where(a => a.GALId == id).OrderByDescending(a => a.Code)
                    .Select(a => new
                    {
                        Code = Convert.ToInt32(a.Code),
                    }).FirstOrDefault();

                if (data != null)
                {
                    if (data.Code < 9)
                    {
                        code = "0" + (data.Code + 1).ToString();
                    }
                    else
                    {
                        code = (data.Code + 1).ToString();
                    }
                }

                json.Value = code;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }

        public static JsonResult Add(TMSContext context, AccountVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            var gal = context.GALs.Where(a => a.Name == "TRADE AND OTHER PAYABLES").FirstOrDefault();
            if (gal == null)
            {
                json.Value = "Error! Supplier GAL not found";
                return json;
            }
            if (GetMaxCode(context, gal.Id).StatusCode != 200)
            {
                json.Value = "Error! Invalid code";
                return json;
            }
            if (string.IsNullOrEmpty(vm.UserId))
            {
                json.Value = "Error! User id cannot be empty";
                return json;
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    Account data = new Account();
                    data.GALId = gal.Id;
                    data.GALCode = gal.CTLCode + gal.Code;
                    data.Code = GetMaxCode(context, gal.Id).Value.ToString();
                    data.Name = vm.Name;
                    data.IsActive = true;
                    data.UserId = vm.UserId;

                    context.Accounts.Add(data);
                    context.SaveChanges();

                    AccountDetail cd = new AccountDetail();
                    cd.AccountId = data.Id;
                    cd.Contact = vm.Contact;
                    cd.CNIC = vm.CNIC;
                    cd.Address = null;
                    cd.CityId = context.Cities.Where(a => a.Name == "Faisalabad").FirstOrDefault().Id;

                    context.AccountDetails.Add(cd);
                    context.SaveChanges();

                    trans.Commit();
                    json.Value = data.Id;
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
    }
}
