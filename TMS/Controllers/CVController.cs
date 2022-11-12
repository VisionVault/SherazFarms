using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class CVController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly DocType CVDt;

        public CVController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            CVDt = context.DocTypes.Where(a => a.Name == "CV").FirstOrDefault();
        }

        public JsonResult GetMaxDocId()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                long docId = 0;
                var data = context.LedgerDetails.Where(a => a.DocTypeId == CVDt.Id).OrderByDescending(a => a.DocId).FirstOrDefault();

                if (data != null)
                {
                    docId = data.DocId + 1;
                }
                else
                {
                    docId = 1;
                }

                json.Value = docId;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        public JsonResult Search(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.Accounts.Where(a => a.IsActive == true)
                    .Select(a => new
                    {
                        a.Id,
                        a.Code,
                        a.Name,
                    }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    code = context.Accounts.Where(a => a.IsActive == true && a.Name.ToLower().Contains(param.ToLower()))
                    .Select(a => new
                    {
                        a.Id,
                        a.Code,
                        a.Name,
                    }).ToList();
                }
                json.Value = code;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        [HttpPost]
        public JsonResult GetAll(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
            {
                json.Value = "Error! Invalid from date";
                return json;
            }
            if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
            {
                json.Value = "Error! Invalid to date";
                return json;
            }
            DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
            DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
            try
            {
                var code = context.LedgerDetails.Where(a => a.TransactionDate >= fromDate && a.TransactionDate <= toDate
                && a.DocTypeId == CVDt.Id && a.TransactionStatusId != 3)
                    .Select(a => new
                    {
                        a.DocId,
                        a.TransactionDate,
                        Date = a.TransactionDate.ToString("dd/MM/yyyy"),
                        Account = a.Account.Name,
                        Debit = a.Debit,
                        Credit = a.Credit
                    }).OrderByDescending(a => a.TransactionDate).ThenByDescending(a => a.DocId).ToList();
                json.Value = code;
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        public JsonResult Recall(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == id && a.DocTypeId == CVDt.Id && a.TransactionStatusId != 3)
                    .Select(a => new
                    {
                        a.DocId,
                        a.TransactionDate,
                        Date = a.TransactionDate.ToString("yyyy-MM-dd"),
                        a.AccountId,
                        Account = a.Account.Name,
                        a.Narration,
                        Debit = a.Debit,
                        Credit = a.Credit,
                    }).ToList();

                json.Value = data;
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
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
            ViewBag.DocId = Convert.ToInt64(GetMaxDocId().Value);
            ViewBag.Date = SysAcc.GetLocalDate().ToString("yyyy-MM-dd");
            return View();
        }
        [HttpPost]
        public JsonResult Add(List<VoucherVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if(vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            if(vm.Sum(a => a.Debit) != vm.Sum(a => a.Credit))
            {
                json.Value = "Error! Debit and credit must be equal";
                return json;
            }
            VoucherVM doc = vm.FirstOrDefault();
            int line = 0;
            foreach(var item in vm)
            {
                line += 1;
                if(item.AccountId == 0)
                {
                    json.Value = "Error! Invalid account on line " + line.ToString();
                    return json;
                }
                if (string.IsNullOrEmpty(item.Narration))
                {
                    json.Value = "Error! Invalid narration on line " + line.ToString();
                    return json;
                }
                if (item.Debit == 0 && item.Credit == 0)
                {
                    json.Value = "Error! Debit and credit are zero on line " + line.ToString();
                    return json;
                }
                if (item.Debit > 0 && item.Credit > 0)
                {
                    json.Value = "Error! Please enter either debit or credit on line " + line.ToString();
                    return json;
                }
            }
            var maxDoc = GetMaxDocId();
            if(maxDoc.StatusCode != 200)
            {
                return maxDoc;
            }
            try
            {
                doc.DocId = Convert.ToInt64(maxDoc.Value);
                doc.DocTypeId = CVDt.Id;
                doc.PostDate = SysAcc.GetLocalDate();
                doc.TransactionStatusId = 1;
                doc.UserId = usrMgr.GetUserId(User);

                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    var ledger = Ledger(vm);
                    if(ledger.StatusCode != 200)
                    {
                        return ledger;
                    }

                    trans.Commit();
                    json.Value = doc.DocId;
                    json.StatusCode = 200;
                    return json;
                }
            }
            catch(Exception ex)
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
        public JsonResult Edit(List<VoucherVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            if (vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            if (vm.Sum(a => a.Debit) != vm.Sum(a => a.Credit))
            {
                json.Value = "Error! Debit and credit must be equal";
                return json;
            }
            VoucherVM doc = vm.FirstOrDefault();
            if (doc.DocId == 0)
            {
                json.Value = "Error! Invalid doc id";
                return json;
            }
            int line = 0;
            foreach (var item in vm)
            {
                line += 1;
                if (item.AccountId == 0)
                {
                    json.Value = "Error! Invalid account on line " + line.ToString();
                    return json;
                }
                if (string.IsNullOrEmpty(item.Narration))
                {
                    json.Value = "Error! Invalid narration on line " + line.ToString();
                    return json;
                }
                if (item.Debit == 0 && item.Credit == 0)
                {
                    json.Value = "Error! Debit and credit are zero on line " + line.ToString();
                    return json;
                }
                if (item.Debit > 0 && item.Credit > 0)
                {
                    json.Value = "Error! Please enter either debit or credit on line " + line.ToString();
                    return json;
                }
            }
            try
            {
                doc.DocTypeId = CVDt.Id;
                doc.PostDate = SysAcc.GetLocalDate();
                doc.TransactionStatusId = 1;
                doc.UserId = usrMgr.GetUserId(User);

                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    var delMsg = Delete(doc.DocId);
                    if (delMsg.StatusCode != 200)
                    {
                        return delMsg;
                    }
                    var ledger = Ledger(vm);
                    if (ledger.StatusCode != 200)
                    {
                        return ledger;
                    }

                    trans.Commit();
                    json.Value = doc.DocId;
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
        public JsonResult Remove(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == id && a.DocTypeId == CVDt.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.TransactionStatusId = 3;
                        context.SaveChanges();
                    }
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }

        public JsonResult Ledger(List<VoucherVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                VoucherVM doc = vm.FirstOrDefault();
                foreach (var item in vm)
                {
                    LedgerDetail ld = new LedgerDetail();
                    ld.DocId = doc.DocId;
                    ld.DocTypeId = doc.DocTypeId;
                    ld.TransactionDate = doc.TransactionDate;
                    ld.PostDate = doc.PostDate;
                    ld.AccountId = item.AccountId;
                    ld.AgainstAccountId = null;
                    ld.Narration = item.Narration;
                    ld.NarrationDetailed = item.Narration;
                    ld.Debit = item.Debit;
                    ld.Credit = item.Credit;
                    ld.TransactionStatusId = doc.TransactionStatusId;
                    ld.UserId = doc.UserId;

                    context.LedgerDetails.Add(ld);
                    context.SaveChanges();
                }
                json.Value = "Success";
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        public JsonResult Delete(long docId)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.LedgerDetails.Where(a => a.DocId == docId && a.DocTypeId == CVDt.Id && a.TransactionStatusId != 3).ToList();
                if (data.Count > 0)
                {
                    context.LedgerDetails.RemoveRange(data);
                    context.SaveChanges();
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
    }
}
