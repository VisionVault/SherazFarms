using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Data;

namespace TMS.Controllers
{
    [Authorize]
    public class CarBrandController : Controller
    {
        private readonly TMSContext context;

        public CarBrandController(TMSContext context)
        {
            this.context = context;
        }

        public JsonResult Search(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.CarBrands
                    .Select(a => new
                    {
                        a.Id,
                        Name = a.Car.Name + " " + a.Name + " " + a.Model + " " + a.Year,
                    }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    data = context.CarBrands.Where(a => a.Name.ToLower().Contains(param.ToLower()))
                    .Select(a => new
                    {
                        a.Id,
                        Name = a.Car.Name + " " + a.Name + " " + a.Model + " " + a.Year,
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
                var code = context.CarBrands
                    .Select(a => new
                    {
                        a.Id,
                        a.CarId,
                        Car = a.Car.Name,
                        a.Name,
                        a.Model,
                        a.Year,
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
        public JsonResult Recall(int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.CarBrands.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.CarId,
                        Car = a.Car.Name,
                        a.Name,
                        InitialName = a.Name,
                        a.Model,
                        a.Year,
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

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Add(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (!int.TryParse(vm["CarId"].ToString(), out int carId))
            {
                json.Value = "Error! Select car from list";
                return json;
            }
            if (string.IsNullOrEmpty(vm["Name"].ToString()))
            {
                json.Value = "Error! Name cannot be empty";
                return json;
            }
            string name = vm["Name"].ToString();
            string model = vm["Model"].ToString();
            string year = vm["Year"].ToString();
            //var exists = Exists(name);
            //if (exists.StatusCode != 200)
            //{
            //    return exists;
            //}
            try
            {
                CarBrand data = new CarBrand();
                data.CarId = carId;
                data.Name = name;
                data.Model = model;
                data.Year = year;

                context.CarBrands.Add(data);
                context.SaveChanges();

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

        public IActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Edit(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (!int.TryParse(vm["Id"].ToString(), out _))
            {
                json.Value = "Error! Invalid Id";
                return json;
            }
            if (!int.TryParse(vm["CarId"].ToString(), out int carId))
            {
                json.Value = "Error! Select car from list";
                return json;
            }
            if (string.IsNullOrEmpty(vm["Name"].ToString()))
            {
                json.Value = "Error! Name cannot be empty";
                return json;
            }
            int id = Convert.ToInt32(vm["Id"]);
            string name = vm["Name"].ToString();
            string initialName = vm["InitialName"].ToString();
            string model = vm["Model"].ToString();
            string year = vm["Year"].ToString();
            if (initialName != name)
            {
                //var exists = Exists(name);
                //if (exists.StatusCode != 200)
                //{
                //    return exists;
                //}
            }
            try
            {
                CarBrand data = context.CarBrands.Where(a => a.Id == id).FirstOrDefault();
                data.CarId = carId;
                data.Name = name;
                data.Model = model;
                data.Year = year;

                context.SaveChanges();

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

        public JsonResult Exists(string name)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.CarBrands.Where(a => a.Name == name).FirstOrDefault();
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
