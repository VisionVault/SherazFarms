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
    public class CTLController : Controller
    {
        private readonly TMSContext context;

        public CTLController(TMSContext context)
        {
            this.context = context;
        }

        public JsonResult GetFSLCode(int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                string code = null;
                var data = context.FSLs.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        Code = a.Code,
                    }).FirstOrDefault();

                if (data != null)
                {
                    code = data.Code;
                    json.Value = code;
                    json.StatusCode = 200;
                    return json;
                }

                json.Value = "Error!";
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
        public JsonResult GetMaxCode(int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                string code = "01";
                var data = context.CTLs.Where(a => a.FSLId == id).OrderByDescending(a => a.Code)
                    .Select(a => new
                    {
                        Code = Convert.ToInt32(a.Code),
                    }).FirstOrDefault();

                if(data != null)
                {
                    if(data.Code < 9)
                    {
                        code =  "0" + (data.Code + 1).ToString();
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
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
        public JsonResult Search(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.CTLs.Where(a => a.Name.ToLower().Contains(param.ToLower()))
                    .Select(a => new
                    {
                        a.Id,
                        a.Code,
                        a.Name,
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
        public JsonResult GetAll()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.CTLs
                    .Select(a => new
                    {
                        a.Id,
                        Code = a.FSLCode + a.Code,
                        a.Name,
                    }).OrderBy(a => Convert.ToInt32(a.Code)).ToList();
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
                var code = context.CTLs.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.FSLId,
                        a.FSLCode,
                        FSL = a.FSL.Name,
                        a.Code,
                        a.Name,
                        InitialName = a.Name,
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
            if(!int.TryParse(vm["FSLId"].ToString(), out _))
            {
                json.Value = "Error! Select FSL from list";
                return json;
            }
            int fslId = Convert.ToInt32(vm["FSLId"]);
            if (GetFSLCode(fslId).StatusCode != 200)
            {
                json.Value = "Error! Invalid FSL code";
                return json;
            }
            if (GetMaxCode(fslId).StatusCode != 200)
            {
                json.Value = "Error! Invalid code";
                return json;
            }
            if (string.IsNullOrEmpty(vm["Name"].ToString()))
            {
                json.Value = "Error! Name cannot be empty";
                return json;
            }
            string name = vm["Name"].ToString();
            var exists = Exists(name);
            if(exists.StatusCode != 200)
            {
                return exists;
            }
            try
            {
                CTL data = new CTL();
                data.FSLId = fslId;
                data.FSLCode = GetFSLCode(fslId).Value.ToString();
                data.Code = GetMaxCode(fslId).Value.ToString();
                data.Name = name;

                context.CTLs.Add(data);
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
            if(!int.TryParse(vm["Id"].ToString(), out _))
            {
                json.Value = "Error! Invalid Id";
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
            if(initialName != name)
            {
                var exists = Exists(name);
                if (exists.StatusCode != 200)
                {
                    return exists;
                }
            }
            try
            {
                CTL data = context.CTLs.Where(a => a.Id == id).FirstOrDefault();
                data.Name = name;

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
                var data = context.CTLs.Where(a => a.Name == name).FirstOrDefault();
                if(data == null)
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
