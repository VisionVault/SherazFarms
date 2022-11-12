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
    public static class CustomerManager
    {
        public static JsonResult AddCustomer(TMSContext context, AccountVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            var gal = context.GALs.Where(a => a.Name == "TRADE DEBTS").FirstOrDefault();
            if(gal == null)
            {
                json.Value = "Error! Customer GAL not found";
                return json;
            }
            string galCode = gal.CTLCode + gal.Code;
            string code = "01";
            var maxCode = context.Accounts.Where(a => a.GALId == gal.Id).OrderByDescending(a => Convert.ToInt32(a.Code)).FirstOrDefault();
            if(maxCode != null)
            {
                int accCode = Convert.ToInt32(maxCode.Code);
                if(accCode < 9)
                {
                    code = "0" + (accCode + 1).ToString();
                }
                else
                {
                    code = (accCode + 1).ToString();
                }
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
                    data.GALCode = galCode;
                    data.Code = code;
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

                    vm.Id = data.Id;
                    json.Value = vm;
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
