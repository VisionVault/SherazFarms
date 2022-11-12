using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class CustomerCarController : Controller
    {
        private readonly TMSContext context;

        public CustomerCarController(TMSContext context)
        {
            this.context = context;
        }

        public JsonResult GetAll()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from a in context.CustomerCars
                            let acc = context.AccountDetails.Where(b => b.AccountId == a.AccountId).FirstOrDefault()
                            select new
                            {
                                a.Id,
                                a.AccountId,
                                Account = a.Account.Name,
                                Contact = acc.Contact,
                                a.RegistrationNumber,
                                a.Color,
                            }).OrderBy(a => a.Account).ToList();
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
        public JsonResult Search(string param, int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from cc in context.CustomerCars.Where(a => a.AccountId == id)
                            let cus = context.AccountDetails.Where(a => a.AccountId == cc.AccountId).FirstOrDefault()
                            select new
                            {
                                Id = cc.Id,
                                cc.Account.Name,
                                Contact = cus.Contact,
                                cc.RegistrationNumber,
                                cc.Color
                            }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    code = (from cc in context.CustomerCars.Where(a => a.AccountId == id)
                            let cus = context.AccountDetails.Where(a => a.AccountId == cc.AccountId).FirstOrDefault()
                            select new
                            {
                                Id = cc.Id,
                                cc.Account.Name,
                                Contact = cus.Contact,
                                cc.RegistrationNumber,
                                cc.Color
                            }).Where(a => (a.Name + a.Contact + a.RegistrationNumber + a.Color).ToLower().Contains(param.ToLower())).ToList();
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
        public JsonResult Recall(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from a in context.CustomerCars.Where(a => a.AccountId == id)
                            let acc = context.AccountDetails.Where(b => b.AccountId == a.AccountId).FirstOrDefault()
                            select new
                            {
                                a.Id,
                                a.AccountId,
                                Account = a.Account.Name,
                                Contact = acc.Contact,
                                a.RegistrationNumber,
                                InitialRegistrationNumber = a.RegistrationNumber,
                                a.Color,
                            }).OrderBy(a => a.Account).ToList();
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

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Add(List<CustomerCarVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            CustomerCarVM doc = vm.FirstOrDefault();
            if (doc.AccountId == 0)
            {
                json.Value = "Error! Please select a customer from list";
                return json;
            }
            if (string.IsNullOrEmpty(doc.RegistrationNumber))
            {
                json.Value = "Error! Please enter registration number";
                return json;
            }
            var exists = Exists(doc.RegistrationNumber);
            if (exists.StatusCode != 200)
            {
                return exists;
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    foreach (var item in vm)
                    {
                        CustomerCar data = new CustomerCar();
                        data.AccountId = doc.AccountId;
                        data.RegistrationNumber = item.RegistrationNumber;
                        data.Color = item.Color;

                        context.CustomerCars.Add(data);
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
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public IActionResult Edit(long id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Edit(List<CustomerCarVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            CustomerCarVM doc = vm.FirstOrDefault();
            if (doc.AccountId == 0)
            {
                json.Value = "Error! Please select a customer from list";
                return json;
            }
            if (string.IsNullOrEmpty(doc.RegistrationNumber))
            {
                json.Value = "Error! Please enter registration number";
                return json;
            }
            if (doc.InitialRegistrationNumber != doc.RegistrationNumber)
            {
                //var exists = Exists(doc.RegistrationNumber);
                //if (exists.StatusCode != 200)
                //{
                //    return exists;
                //}
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    foreach (var item in vm)
                    {
                        CustomerCar data = new CustomerCar();
                        if(item.Id > 0)
                        {
                            data = context.CustomerCars.Where(a => a.Id == item.Id).FirstOrDefault();
                        }
                        data.AccountId = doc.AccountId;
                        data.RegistrationNumber = item.RegistrationNumber;
                        data.Color = item.Color;

                        if (item.Id == 0)
                        {
                            context.CustomerCars.Add(data);
                        }
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
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public JsonResult Exists(string name)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.CustomerCars.Where(a => a.RegistrationNumber == name).FirstOrDefault();
                if (data == null)
                {
                    json.Value = "Success";
                    json.StatusCode = 200;
                    return json;
                }

                json.Value = "Error! Entity already exists";
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public JsonResult Delete(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.CustomerCars.Where(a => a.Id == id).FirstOrDefault();
                if (data != null)
                {
                    context.CustomerCars.Remove(data);
                    context.SaveChanges();
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + "  " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }
    }
}
