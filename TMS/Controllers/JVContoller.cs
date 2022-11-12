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
    public class JVController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly DocType JVDT;
        private readonly GAL CashGAL;
        private readonly GAL BankGAL;

        public JVController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            JVDT = context.DocTypes.Where(a => a.Name == "JV").FirstOrDefault();
            CashGAL = context.GALs.Where(a => a.Name == "Cash in Hand").FirstOrDefault();
            BankGAL = context.GALs.Where(a => a.Name == "Cash at Bank").FirstOrDefault();
        }

        public JsonResult GetMaxDocId()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                long docId = 0;
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocTypeId == JVDT.Id).OrderByDescending(a => a.DocId).FirstOrDefault();

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
                var code = context.Accounts.Where(a => a.GALId != CashGAL.Id && a.GALId != BankGAL.Id && a.Name.ToLower().Contains(param.ToLower()))
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
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var code = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.TransactionDate >= fromDate && a.TransactionDate <= toDate
                && a.DocTypeId == JVDT.Id && a.TransactionStatusId != 3 && a.Credit > 0)
                    .Select(a => new
                    {
                        a.DocId,
                        a.TransactionDate,
                        Date = a.TransactionDate.ToString("dd/MM/yyyy"),
                        Account = a.Account.Name,
                        AgainstAccount = a.AgainstAccount == null ? "" : a.AgainstAccount.Name,
                        Amount = a.Credit
                    }).OrderByDescending(a => a.TransactionDate).ThenByDescending(a => a.DocId).ToList();
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
        public JsonResult Recall(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocId == id && a.DocTypeId == JVDT.Id && a.TransactionStatusId != 3 && a.Debit > 0)
                    .Select(a => new
                    {
                        a.DocId,
                        a.TransactionDate,
                        Date = a.TransactionDate.ToString("yyyy-MM-dd"),
                        DebitAcId = a.AccountId,
                        DebitAc = a.Account.Name,
                        CreditAcId = a.AgainstAccountId,
                        CreditAc = a.AgainstAccount.Name,
                        a.Narration,
                        Amount = a.Debit
                    }).ToList();

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
            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            vm.FirstOrDefault().TradeFirmId = tf.Id;
            vm.FirstOrDefault().TransactionStatusId = 1;
            vm.FirstOrDefault().UserId = usrMgr.GetUserId(User);

            var led = Ledger(vm);
            return led;
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

            if(vm.FirstOrDefault().DocId == 0)
            {
                json.Value = "Error! Invalid voucher id";
                return json;
            }

            var delMsg = Delete(vm.FirstOrDefault().DocId);
            if(delMsg.StatusCode != 200)
            {
                return delMsg;
            }

            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            vm.FirstOrDefault().TradeFirmId = tf.Id;
            vm.FirstOrDefault().TransactionStatusId = 2;
            vm.FirstOrDefault().UserId = usrMgr.GetUserId(User);

            var led = Ledger(vm);
            return led;
        }
        public JsonResult Remove(long id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocId == id && a.DocTypeId == JVDT.Id && a.TransactionStatusId != 3).ToList();
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
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public JsonResult Ledger(List<VoucherVM> vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            int line = 0;
            if(vm.Count == 0)
            {
                json.Value = "Error! No data found";
                return json;
            }
            VoucherVM doc = vm.FirstOrDefault();
            doc.DocId = Convert.ToInt64(GetMaxDocId().Value);
            doc.DocTypeId = JVDT.Id;
            foreach(var item in vm)
            {
                line += 1;
                if(item.DebitAcId == 0)
                {
                    json.Value = "Error! Invalid debit account id on line number " + line.ToString();
                    return json;
                }
                if (item.CreditAcId == 0)
                {
                    json.Value = "Error! Invalid credit account id on line number " + line.ToString();
                    return json;
                }
                if (item.Amount == 0)
                {
                    json.Value = "Error! invalid amount on " + line.ToString();
                    return json;
                }
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    foreach(var item in vm)
                    {
                        for(int i = 0; i < 2; i++)
                        {
                            LedgerDetail ld = new LedgerDetail();
                            ld.DocId = doc.DocId;
                            ld.DocTypeId = doc.DocTypeId;
                            ld.TransactionDate = doc.TransactionDate;
                            ld.PostDate = SysAcc.GetLocalDate();
                            if(i == 0)
                            {
                                ld.AccountId = item.DebitAcId;
                                ld.AgainstAccountId = item.CreditAcId;
                                ld.Narration = item.Narration;
                                ld.NarrationDetailed = item.Narration;
                                ld.Debit = item.Amount;
                                ld.Credit = 0;
                            }
                            if (i == 1)
                            {
                                ld.AccountId = item.CreditAcId;
                                ld.AgainstAccountId = item.DebitAcId;
                                ld.Narration = item.Narration;
                                ld.NarrationDetailed = item.Narration;
                                ld.Debit = 0;
                                ld.Credit = item.Amount;
                            }
                            ld.TransactionStatusId = doc.TransactionStatusId;
                            ld.TradeFirmId = doc.TradeFirmId;
                            ld.UserId = doc.UserId;

                            context.LedgerDetails.Add(ld);
                            context.SaveChanges();
                        }
                    }

                    trans.Commit();
                    json.Value = doc.DocId;
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
        public JsonResult Delete(long docId)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                var data = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocId == docId && a.DocTypeId == JVDT.Id && a.TransactionStatusId != 3).ToList();
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
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }
    }
}
