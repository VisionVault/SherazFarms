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
    public class CarEngineController : Controller
    {
        private readonly TMSContext context;

        public CarEngineController(TMSContext context)
        {
            this.context = context;
        }

        public JsonResult Search(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.CarEngines
                    .Select(a => new
                    {
                        a.Id,
                        Name = a.CarBrand.Car.Name + " " + a.CarBrand.Name + " " + a.CarBrand.Model + " " + a.CarBrand.Year + " " + a.EngineNumber,
                    }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    data = context.CarEngines.Where(a =>
                    (a.CarBrand.Car.Name + a.CarBrand.Name + a.CarBrand.Model + a.CarBrand.Year + a.EngineNumber).ToLower().Contains(param.ToLower()))
                    .Select(a => new
                    {
                        a.Id,
                        Name = a.CarBrand.Car.Name + " " + a.CarBrand.Name + " " + a.CarBrand.Model + " " + a.CarBrand.Year + " " + a.EngineNumber,
                    }).ToList();
                }
                json.Value = data;
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
                var code = context.CarEngines
                    .Select(a => new
                    {
                        a.Id,
                        CarBrand = a.CarBrand.Car.Name + ' ' + a.CarBrand.Name + " " + a.CarBrand.Model + " " + a.CarBrand.Year,
                        a.EngineNumber,
                        a.EngineCC,
                        a.OilCapacity,
                        a.Company,
                        a.OilFilter,
                        a.AirFilter,
                        a.FuelFilter,
                        a.CabinFilter
                    }).OrderBy(a => a.EngineNumber).ToList();
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
        public JsonResult Recall(string id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.CarEngines.Where(a => a.EngineNumber == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.CarBrandId,
                        CarBrand = a.CarBrand.Car.Name + ' ' + a.CarBrand.Name + " " + a.CarBrand.Model + " " + a.CarBrand.Year,
                        a.EngineNumber,
                        InitialEngineNumber = a.EngineNumber,
                        a.EngineCC,
                        a.OilCapacity,
                        a.Company,
                        a.OilFilter,
                        a.AirFilter,
                        a.FuelFilter,
                        a.CabinFilter
                    }).ToList();
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
        public JsonResult Add(List<CarEngineVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if(vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            CarEngineVM doc = vm.FirstOrDefault();
            if(!int.TryParse(doc.CarBrandId.ToString(), out int carBrandId) || carBrandId == 0)
            {
                json.Value = "Error! Please select a car brand from list";
                return json;
            }
            if(string.IsNullOrEmpty(doc.EngineNumber))
            {
                json.Value = "Error! Please enter engine number";
                return json;
            }
            var exists = Exists(carBrandId, doc.EngineNumber);
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
                        CarEngine data = new CarEngine();
                        data.CarBrandId = carBrandId;
                        data.EngineNumber = doc.EngineNumber;
                        data.EngineCC = doc.EngineCC;
                        data.OilCapacity = doc.OilCapacity;
                        data.Company = item.Company;
                        data.OilFilter = item.OilFilter;
                        data.AirFilter = item.AirFilter;
                        data.FuelFilter = item.FuelFilter;
                        data.CabinFilter = item.CabinFilter;

                        context.CarEngines.Add(data);
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

        public IActionResult Edit(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Edit(List<CarEngineVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            CarEngineVM doc = vm.FirstOrDefault();
            if (!int.TryParse(doc.CarBrandId.ToString(), out int carBrandId) || carBrandId == 0)
            {
                json.Value = "Error! Please select a car brand from list";
                return json;
            }
            if (string.IsNullOrEmpty(doc.EngineNumber))
            {
                json.Value = "Error! Please enter engine number";
                return json;
            }
            if(doc.InitialEngineNumber != doc.EngineNumber)
            {
                var exists = Exists(carBrandId, doc.EngineNumber);
                if (exists.StatusCode != 200)
                {
                    return exists;
                }
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    var oldData = context.CarEngines.Where(a => a.CarBrandId == carBrandId && a.EngineNumber == doc.EngineNumber)
                        .ToList();
                    if (oldData.Count > 0)
                    {
                        context.CarEngines.RemoveRange(oldData);
                        context.SaveChanges();
                    }
                    foreach (var item in vm)
                    {
                        CarEngine data = new CarEngine();
                        data.CarBrandId = carBrandId;
                        data.EngineNumber = doc.EngineNumber;
                        data.EngineCC = doc.EngineCC;
                        data.OilCapacity = doc.OilCapacity;
                        data.Company = item.Company;
                        data.OilFilter = item.OilFilter;
                        data.AirFilter = item.AirFilter;
                        data.FuelFilter = item.FuelFilter;
                        data.CabinFilter = item.CabinFilter;

                        context.CarEngines.Add(data);
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

        public JsonResult Exists(int? cbId, string name)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.CarEngines.Where(a => a.CarBrandId == cbId && a.EngineNumber == name).FirstOrDefault();
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
    }
}
