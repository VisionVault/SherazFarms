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
    public class GALController : Controller
    {
        private readonly TMSContext context;

        public GALController(TMSContext context)
        {
            this.context = context;
        }

        public JsonResult GetCTLCode(int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                string code = null;
                var data = context.CTLs.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        Code = a.FSLCode + a.Code,
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
                var data = context.GALs.Where(a => a.CTLId == id).OrderByDescending(a => a.Code)
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
                var code = context.GALs.Where(a => a.Name.ToLower().Contains(param.ToLower()))
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
                var code = context.GALs
                    .Select(a => new
                    {
                        a.Id,
                        Code = a.CTLCode + a.Code,
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
                var code = context.GALs.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.CTLId,
                        a.CTLCode,
                        CTL = a.CTL.Name,
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
            if(!int.TryParse(vm["CTLId"].ToString(), out _))
            {
                json.Value = "Error! Select CTL from list";
                return json;
            }
            int CTLId = Convert.ToInt32(vm["CTLId"]);
            if (GetCTLCode(CTLId).StatusCode != 200)
            {
                json.Value = "Error! Invalid CTL code";
                return json;
            }
            if (GetMaxCode(CTLId).StatusCode != 200)
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
                GAL data = new GAL();
                data.CTLId = CTLId;
                data.CTLCode = GetCTLCode(CTLId).Value.ToString();
                data.Code = GetMaxCode(CTLId).Value.ToString();
                data.Name = name;

                context.GALs.Add(data);
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
                GAL data = context.GALs.Where(a => a.Id == id).FirstOrDefault();
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
                var data = context.GALs.Where(a => a.Name == name).FirstOrDefault();
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
