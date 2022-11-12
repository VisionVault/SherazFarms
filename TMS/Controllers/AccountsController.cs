using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TMS.Data;
using TMS.VMs;

namespace TMS.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly DocType OBDt;
        private readonly DocType PurDT;
        private readonly DocType PurRetDT;
        private readonly DocType SaleDT;
        private readonly GAL CashGAL;
        private readonly GAL BankGAL;
        private readonly DocType SaleRetDT;
        private readonly Account CapitalAc;

        public AccountsController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            OBDt = context.DocTypes.Where(a => a.Name == "OB").FirstOrDefault();
            PurDT = context.DocTypes.Where(a => a.Name == "PV").FirstOrDefault();
            PurRetDT = context.DocTypes.Where(a => a.Name == "PRV").FirstOrDefault();
            SaleDT = context.DocTypes.Where(a => a.Name == "SV").FirstOrDefault();
            CashGAL = context.GALs.Where(a => a.Name == "Cash in Hand").FirstOrDefault();
            BankGAL = context.GALs.Where(a => a.Name == "Cash at Bank").FirstOrDefault();
            SaleRetDT = context.DocTypes.Where(a => a.Name == "SRV").FirstOrDefault();
            CapitalAc = context.Accounts.Where(a => a.Name == "CAPITAL").FirstOrDefault();
        }

        public JsonResult GetGALCode(int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                string code = null;
                var data = context.GALs.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        Code = a.CTLCode + a.Code,
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        public JsonResult Search(string param, int galId)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                Expression<Func<Account, bool>> pred;
                if (galId == 0)
                {
                    pred = a => a.Name.ToLower().Contains(param.ToLower());
                }
                else
                {
                    pred = a => a.GALId == galId && a.Name.ToLower().Contains(param.ToLower());
                }
                var code = (from a in context.Accounts.Where(a => a.IsActive == true)
                            let acc = context.AccountDetails.Where(b => b.AccountId == a.Id)
                            .Select(b => new
                            {
                                b.Contact,
                                b.Address,
                                City = b.City == null ? "" : b.City.Name,

                            }).FirstOrDefault()
                            select new
                            {
                                a.Id,
                                a.Code,
                                Name = a.Name + " " + (acc == null ? "" : acc.Contact + " " + acc.City),
                            }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    code = (from a in context.Accounts.Where(pred)
                            let acc = context.AccountDetails.Where(b => b.AccountId == a.Id)
                            .Select(b => new
                            {
                                b.Contact,
                                b.Address,
                                City = b.City == null ? "" : b.City.Name,

                            }).FirstOrDefault()
                            select new
                            {
                                a.Id,
                                a.Code,
                                Name = a.Name + " " + (acc == null ? "" : acc.Contact + " " + acc.City),
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
        public JsonResult SearchCashAC(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.Accounts.Where(a => a.GALId == CashGAL.Id && a.Name.ToLower().Contains(param.ToLower()))
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        public JsonResult SearchBankAC(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.Accounts.Where(a => a.GALId == BankGAL.Id && a.Name.ToLower().Contains(param.ToLower()))
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }
        public JsonResult GetAll()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.Accounts
                    .Select(a => new
                    {
                        a.Id,
                        GAL = a.GAL.Name,
                        Code = a.GALCode + a.Code,
                        a.Name,
                        a.IsActive,
                    }).OrderBy(a => Convert.ToInt32(a.Code)).ToList();
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
        public JsonResult GetById(long param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from a in context.AccountDetails.Where(a => a.AccountId == param && a.Account.IsActive == true)
                            select new
                            {
                                Id = a.AccountId,
                                a.Account.Name,
                                a.Contact,
                                City = a.City.Name,
                                Balance = context.LedgerDetails.Where(b => b.TransactionStatusId != 3 && b.TradeFirmId == 1 && b.AccountId == a.Id).Sum(b => b.Debit - b.Credit)
                            }).FirstOrDefault();
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
        public JsonResult GetByContact(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from a in context.AccountDetails.Where(a => a.Contact == param && a.Account.IsActive == true)
                            select new
                            {
                                Id = a.AccountId,
                                a.Account.Name,
                                a.Contact,
                                City = a.City.Name,
                                Balance = context.LedgerDetails.Where(b => b.TransactionStatusId != 3 && b.TradeFirmId == 1 && b.AccountId == a.Id).Sum(b => b.Debit - b.Credit)
                            }).FirstOrDefault();
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
        public JsonResult GetByCar(long param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from a in context.CustomerCars.Where(a => a.Id == param)

                            let cus = context.AccountDetails.Where(b => b.AccountId == a.AccountId).FirstOrDefault()
                            select new
                            {
                                Id = a.AccountId,
                                a.Account.Name,
                                cus.Contact,
                                a.RegistrationNumber,
                                a.Color
                            }).FirstOrDefault();
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
        public JsonResult Recall(int id)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from a in context.Accounts.Where(a => a.Id == id)

                            let ob = context.LedgerDetails.Where(b => b.AccountId == a.Id && b.DocTypeId == OBDt.Id && b.TransactionStatusId != 3).FirstOrDefault()
                            select new
                            {
                                a.Id,
                                a.GALId,
                                a.GALCode,
                                GAL = a.GAL.Name,
                                a.Code,
                                a.Name,
                                InitialName = a.Name,
                                a.IsActive,
                                OpBalance = ob.Debit - ob.Credit
                            }).FirstOrDefault();
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
        public IActionResult Index()
        {
            return View();
        }

        //Reports
        public IActionResult COA()
        {
            try
            {
                List<COAVM> data = context.Accounts.Where(a => a.IsActive == true)
                    .Select(a => new COAVM
                    {
                        CTLCode = a.GAL.CTLCode,
                        CTL = a.GAL.CTL.Name,
                        GALCode = a.GALCode,
                        GAL = a.GAL.Name,
                        AccCode = a.GALCode + a.Code,
                        Code = a.Code,
                        Acc = a.Name
                    }).OrderBy(a => Convert.ToInt64(a.GALCode)).ThenBy(a => Convert.ToInt64(a.Code)).ToList();

                string ctlCode = "";
                foreach (var item in data)
                {
                    if (ctlCode != item.CTLCode)
                    {
                        item.IsCTLChanged = true;
                    }
                    ctlCode = item.CTLCode;
                }

                string galCode = "";
                foreach (var item in data)
                {
                    if (galCode != item.GALCode)
                    {
                        item.IsGALChanged = true;
                    }
                    galCode = item.GALCode;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }
        }
        [HttpPost]
        public IActionResult Trial(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");
                List<RPTTrialVM> data = context.Accounts.Where(a => a.IsActive == true)
                    .Select(a => new RPTTrialVM
                    {
                        CTLCode = a.GAL.CTLCode,
                        CTL = a.GAL.CTL.Name,
                        GALCode = a.GALCode,
                        GAL = a.GAL.Name,
                        AccountId = a.Id,
                        AccCode = a.GALCode + a.Code,
                        Code = a.Code,
                        Acc = a.Name
                    }).OrderBy(a => Convert.ToInt64(a.GALCode)).ThenBy(a => Convert.ToInt64(a.Code)).ToList();
                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<RPTTrialVM> opening = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && (a.TransactionDate < fromDate || a.DocTypeId == OBDt.Id) && a.TransactionStatusId != 3)
                                            group new { l.Debit, l.Credit } by l.AccountId into g
                                            select new RPTTrialVM
                                            {
                                                Type = "Opening",
                                                AccountId = g.Key,
                                                Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                                Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                            }).ToList();
                List<RPTTrialVM> current = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.DocTypeId != OBDt.Id && a.TransactionDate >= fromDate && a.TransactionDate <= toDate
                                            && a.TransactionStatusId != 3)
                                            group new { l.Debit, l.Credit } by l.AccountId into g
                                            select new RPTTrialVM
                                            {
                                                Type = "Current",
                                                AccountId = g.Key,
                                                Debit = g.Sum(a => a.Debit),
                                                Credit = g.Sum(a => a.Credit),
                                            }).ToList();
                List<RPTTrialVM> closing = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                            group new { l.Debit, l.Credit } by l.AccountId into g
                                            select new RPTTrialVM
                                            {
                                                Type = "Closing",
                                                AccountId = g.Key,
                                                Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                                Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                            }).ToList();
                data = (from d in data

                        join op in opening
                        on d.AccountId equals op.AccountId into oplj
                        from op in oplj.DefaultIfEmpty()

                        join cur in current
                        on d.AccountId equals cur.AccountId into curlj
                        from cur in curlj.DefaultIfEmpty()

                        join cl in closing
                        on d.AccountId equals cl.AccountId into cllj
                        from cl in cllj.DefaultIfEmpty()

                        select new RPTTrialVM
                        {
                            CTLCode = d.CTLCode,
                            CTL = d.CTL,
                            GALCode = d.GALCode,
                            GAL = d.GAL,
                            AccountId = d.AccountId,
                            AccCode = d.AccCode,
                            Acc = d.Acc,
                            OpDebit = op == null ? 0 : op.Debit,
                            OpCredit = op == null ? 0 : op.Credit,
                            CurrDebit = cur == null ? 0 : cur.Debit,
                            CurrCredit = cur == null ? 0 : cur.Credit,
                            ClDebit = cl == null ? 0 : cl.Debit,
                            ClCredit = cl == null ? 0 : cl.Credit,
                        }).ToList();
                data = data.OrderBy(a => Convert.ToInt64(a.GALCode)).ThenBy(a => Convert.ToInt64(a.AccCode)).ToList();
                string ctlCode = "";
                foreach (var item in data)
                {
                    if (ctlCode != item.CTLCode)
                    {
                        item.IsCTLChanged = true;
                    }
                    ctlCode = item.CTLCode;
                }

                string galCode = "";
                foreach (var item in data)
                {
                    if (galCode != item.GALCode)
                    {
                        item.IsGALChanged = true;
                    }
                    galCode = item.GALCode;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }
        }
        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult Ledger(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                if (!long.TryParse(vm["AccountId"].ToString(), out _) || Convert.ToInt64(vm["AccountId"]) == 0)
                {
                    ViewBag.Error = "Error! Invalid Account Id";
                }
                DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");
                long accountId = Convert.ToInt64(vm["AccountId"]);
                Account acc = context.Accounts.Where(a => a.Id == accountId).FirstOrDefault();
                ViewBag.Account = "";
                if (acc != null)
                {
                    ViewBag.Account = acc.Name;
                }

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<LedgerVM> data = new List<LedgerVM>();
                List<LedgerVM> opening = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == accountId
                    && (a.TransactionDate < fromDate || a.DocTypeId == OBDt.Id) && a.TransactionStatusId != 3)

                                          group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                          select new LedgerVM
                                          {
                                              TransactionDate = fromDate,
                                              Date = fromDate.ToString("dd/MM/yyyy"),
                                              DocId = (long)0,
                                              DocType = "OB",
                                              Account = g.Key.Account,
                                              TRKNumber = "",
                                              Narration = "Opening balance of " + g.Key.Account,
                                              Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                              Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                              Balance = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : g.Sum(a => a.Credit - a.Debit)
                                          }).ToList();
                List<LedgerVM> ledger = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == accountId
                && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.DocTypeId != OBDt.Id && a.TransactionStatusId != 3)
                                         select new LedgerVM
                                         {
                                             TransactionDate = l.TransactionDate,
                                             Date = l.TransactionDate.ToString("dd/MM/yyyy"),
                                             DocId = l.DocId,
                                             DocType = l.DocType.Name,
                                             Account = l.AgainstAccount.Name,
                                             Narration = l.Narration,
                                             Debit = l.Debit,
                                             Credit = l.Credit,
                                             Balance = 0
                                         }).ToList();
                data = opening.Union(ledger).ToList();
                data = data.OrderBy(a => a.TransactionDate).ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }

        }
        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult Receiveables(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<RPTLedgerVM> data = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && (a.Account.GAL.Name == "TRADE DEBTS" || a.Account.GAL.Name == "TRADE AND OTHER PAYABLES") &&
                                          a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                          group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                          select new RPTLedgerVM
                                          {
                                              Account = g.Key.Account,
                                              Debit = g.Sum(a => a.Debit - a.Credit),
                                          }).ToList();
                data = data.Where(a => a.Debit > 0).OrderBy(a => a.Account).ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }

        }
        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult Payables(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<RPTLedgerVM> data = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && (a.Account.GAL.Name == "TRADE DEBTS" || a.Account.GAL.Name == "TRADE AND OTHER PAYABLES") &&
                                          a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                          group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                          select new RPTLedgerVM
                                          {
                                              Account = g.Key.Account,
                                              Debit = g.Sum(a => a.Credit - a.Debit),
                                          }).ToList();
                data = data.Where(a => a.Debit > 0).OrderBy(a => a.Account).ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }
        }
        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult CashBook(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid Date";
                }
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                DateTime fromDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 00, 00, 00);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "As on " + toDate.ToString("dd/MM/yyyy");
                Account acc = context.Accounts.Where(a => a.Name == "Cash in Hand").FirstOrDefault();
                ViewBag.Account = "";
                if (acc != null)
                {
                    ViewBag.Account = acc.Name;
                }

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<LedgerVM> data = new List<LedgerVM>();
                LedgerVM opening = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id
                    && (a.TransactionDate < fromDate || a.DocTypeId == OBDt.Id) && a.TransactionStatusId != 3)

                                    group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                    select new LedgerVM
                                    {
                                        EntryType = "Debit",
                                        DocId = 0,
                                        DocType = "OB",
                                        Account = g.Key.Account,
                                        Narration = "Opening balance",
                                        Balance = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : g.Sum(a => a.Credit - a.Debit),
                                    }).FirstOrDefault();
                data.Add(opening);
                List<LedgerVM> debit = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id && a.Debit > 0
                && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.DocTypeId != OBDt.Id && a.TransactionStatusId != 3)
                                        select new LedgerVM
                                        {
                                            EntryType = "Debit",
                                            DocId = l.DocId,
                                            DocType = l.DocType.Name,
                                            Account = l.AgainstAccount.Name,
                                            Narration = l.Narration,
                                            Balance = l.Debit,
                                        }).ToList();
                foreach (var item in debit)
                {
                    data.Add(item);
                }
                List<LedgerVM> credit = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id && a.Credit > 0
                && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.DocTypeId != OBDt.Id && a.TransactionStatusId != 3)
                                         select new LedgerVM
                                         {
                                             EntryType = "Credit",
                                             DocId = l.DocId,
                                             DocType = l.DocType.Name,
                                             Account = l.AgainstAccount.Name,
                                             Narration = l.Narration,
                                             Balance = l.Credit
                                         }).ToList();
                foreach (var item in credit)
                {
                    data.Add(item);
                }
                LedgerVM closing = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id
                    && (a.TransactionDate <= toDate) && a.TransactionStatusId != 3)

                                    group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                    select new LedgerVM
                                    {
                                        EntryType = "Credit",
                                        DocId = 0,
                                        DocType = "CB",
                                        Account = g.Key.Account,
                                        Narration = "Closing balance",
                                        Balance = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : g.Sum(a => a.Credit - a.Debit),
                                    }).FirstOrDefault();
                data.Add(closing);
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }

        }
        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult BankBook(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid Date";
                }
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                DateTime fromDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 00, 00, 00);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "As on " + toDate.ToString("dd/MM/yyyy");
                List<Account> accounts = context.Accounts.Where(a => a.GAL.Name == "Cash at Bank").ToList();

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<LedgerVM> data = new List<LedgerVM>();
                foreach (var acc in accounts)
                {
                    LedgerVM opening = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id
                    && (a.TransactionDate < fromDate || a.DocTypeId == OBDt.Id) && a.TransactionStatusId != 3)

                                        group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                        select new LedgerVM
                                        {
                                            EntryType = "Debit",
                                            DocId = 0,
                                            DocType = "OB",
                                            Name = context.Accounts.Where(b => b.Id == acc.Id).FirstOrDefault().Name,
                                            Account = g.Key.Account,
                                            Narration = "Opening balance",
                                            Balance = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : g.Sum(a => a.Credit - a.Debit),
                                        }).FirstOrDefault();
                    if (opening != null)
                    {
                        data.Add(opening);
                    }
                    List<LedgerVM> debit = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id && a.Debit > 0
                    && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.DocTypeId != OBDt.Id && a.TransactionStatusId != 3)
                                            select new LedgerVM
                                            {
                                                EntryType = "Debit",
                                                DocId = l.DocId,
                                                DocType = l.DocType.Name,
                                                Name = context.Accounts.Where(b => b.Id == acc.Id).FirstOrDefault().Name,
                                                Account = l.AgainstAccount.Name,
                                                Narration = l.Narration,
                                                Balance = l.Debit,
                                            }).ToList();
                    foreach (var item in debit)
                    {
                        data.Add(item);
                    }
                    List<LedgerVM> credit = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id && a.Credit > 0
                    && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.DocTypeId != OBDt.Id && a.TransactionStatusId != 3)
                                             select new LedgerVM
                                             {
                                                 EntryType = "Credit",
                                                 DocId = l.DocId,
                                                 DocType = l.DocType.Name,
                                                 Name = context.Accounts.Where(b => b.Id == acc.Id).FirstOrDefault().Name,
                                                 Account = l.AgainstAccount.Name,
                                                 Narration = l.Narration,
                                                 Balance = l.Credit
                                             }).ToList();
                    foreach (var item in credit)
                    {
                        data.Add(item);
                    }
                    LedgerVM closing = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.AccountId == acc.Id
                        && (a.TransactionDate <= toDate) && a.TransactionStatusId != 3)

                                        group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                        select new LedgerVM
                                        {
                                            EntryType = "Credit",
                                            DocId = 0,
                                            DocType = "CB",
                                            Name = context.Accounts.Where(b => b.Id == acc.Id).FirstOrDefault().Name,
                                            Account = g.Key.Account,
                                            Narration = "Closing balance",
                                            Balance = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : g.Sum(a => a.Credit - a.Debit),
                                        }).FirstOrDefault();
                    if (closing != null)
                    {
                        data.Add(closing);
                    }
                }
                data = data.OrderBy(a => a.Name).ToList();
                string name = "";
                foreach (var item in data)
                {
                    if (name != item.Name)
                    {
                        item.isGroupChanged = true;
                    }
                    name = item.Name;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }

        }
        [HttpPost, Authorize(Roles = "Admin,Manager")]
        public IActionResult ProfitLoss(IFormCollection vm)
        {
            if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
            {
                ViewBag.Error = "Error! Invalid From Date";
            }
            if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
            {
                ViewBag.Error = "Error! Invalid From Date";
            }
            DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
            DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");

            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            List<RPTPLVM> data = new List<RPTPLVM>();
            List<RPTPLVM> sales = (from p in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.Name == "Sale" && a.TransactionStatusId != 3
                                    && a.TransactionDate >= fromDate && a.TransactionDate <= toDate)
                                   group p.Credit by 1 into g

                                   select new RPTPLVM
                                   {
                                       Sr = 1,
                                       GAL = "Sales",
                                       Account = "Sales",
                                       Debit = g.Sum(),
                                       Credit = 0,
                                   }).ToList();
            List<RPTPLVM> salesReturn = (from p in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.Name == "Sale Return" && a.TransactionStatusId != 3
                                    && a.TransactionDate >= fromDate && a.TransactionDate <= toDate)
                                         group p.Debit by 1 into g

                                         select new RPTPLVM
                                         {
                                             Sr = 2,
                                             GAL = "Sales",
                                             Account = "Sales Return",
                                             Debit = 0,
                                             Credit = g.Sum(),
                                         }).ToList();
            List<RPTPLVM> cos = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.Name == "CGS"
                                         && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                 group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g

                                 select new RPTPLVM
                                 {
                                     Sr = 10,
                                     GAL = "COST OF SALES",
                                     Account = g.Key.Account,
                                     Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                     Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                 }).ToList();
            List<RPTPLVM> adminExp = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && (a.Account.GAL.Name == "ADMINISTRATIVE EXPENSES")
                                        && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                      group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g

                                      select new RPTPLVM
                                      {
                                          Sr = 10,
                                          GAL = "ADMINISTRATIVE EXPENSES",
                                          Account = g.Key.Account,
                                          Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                          Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                      }).ToList();
            List<RPTPLVM> incomes = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.GAL.Name == "OTHER INCOME"
                            && a.TransactionDate >= fromDate && a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                     group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g

                                     select new RPTPLVM
                                     {
                                         Sr = 11,
                                         GAL = "OTHER INCOME",
                                         Account = g.Key.Account,
                                         Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                         Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                     }).ToList();
            data.AddRange(sales);
            data.AddRange(salesReturn);
            data.AddRange(cos);
            data.AddRange(adminExp);
            data.AddRange(incomes);
            data = data.OrderBy(a => a.GAL).ThenBy(a => a.Account).ToList();
            string gal = "";
            foreach (var item in data)
            {
                if (gal != item.GAL)
                {
                    item.IsGroupChanged = true;
                }
                gal = item.GAL;
            }
            data = data.OrderBy(a => a.Sr).ToList();
            return View(data);
        }
        [HttpPost, Authorize(Roles = "Admin,User")]
        public IActionResult BalanceSheet(IFormCollection vm)
        {
            try
            {
                if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
                {
                    ViewBag.Error = "Error! Invalid From Date";
                }
                DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
                DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");

                var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                List<RPTTrialVM> data = new List<RPTTrialVM>();
                List<RPTTrialVM> debit = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.GAL.CTL.FSL.Name == "Balance Sheet" &&
                a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                          group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                          select new RPTTrialVM
                                          {
                                              Type = "Debit",
                                              AccountId = g.Key.AccountId,
                                              Acc = g.Key.Account,
                                              Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                              Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                          }).ToList();
                debit = debit.Where(a => a.Debit > 0).ToList();
                List<RPTTrialVM> credit = (from l in context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.GAL.CTL.FSL.Name == "Balance Sheet" &&
                a.TransactionDate <= toDate && a.TransactionStatusId != 3)
                                           group new { l.Debit, l.Credit } by new { l.AccountId, Account = l.Account.Name } into g
                                           select new RPTTrialVM
                                           {
                                               Type = "Credit",
                                               AccountId = g.Key.AccountId,
                                               Acc = g.Key.Account,
                                               Debit = g.Sum(a => a.Debit - a.Credit) > 0 ? g.Sum(a => a.Debit - a.Credit) : 0,
                                               Credit = g.Sum(a => a.Credit - a.Debit) > 0 ? g.Sum(a => a.Credit - a.Debit) : 0,
                                           }).ToList();
                credit = credit.Where(a => a.Credit > 0).ToList();
                double profit = PandL(vm);
                RPTTrialVM profitAndLoss = new RPTTrialVM
                {
                    Type = profit > 0 ? "Credit" : "Debit",
                    AccountId = 0,
                    Acc = "Profit and Loss",
                    Debit = profit < 0 ? profit * -1 : 0,
                    Credit = profit > 0 ? profit : 0,
                };
                if (profitAndLoss.Type == "Credit")
                {
                    credit.Add(profitAndLoss);
                }
                if (profitAndLoss.Type == "Debit")
                {
                    debit.Add(profitAndLoss);
                }
                data = debit.Union(credit).ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return View();
            }
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
            if (!int.TryParse(vm["GALId"].ToString(), out _))
            {
                json.Value = "Error! Select GAL from list";
                return json;
            }
            int GALId = Convert.ToInt32(vm["GALId"]);
            if (GetGALCode(GALId).StatusCode != 200)
            {
                json.Value = "Error! Invalid GAL code";
                return json;
            }
            if (GetMaxCode(GALId).StatusCode != 200)
            {
                json.Value = "Error! Invalid code";
                return json;
            }
            if (string.IsNullOrEmpty(vm["Name"].ToString()))
            {
                json.Value = "Error! Name cannot be empty";
                return json;
            }
            if (OBDt == null)
            {
                json.Value = "Error! Opening stock Doc Type not found";
                return json;
            }
            if (CapitalAc == null)
            {
                json.Value = "Error! Capital Account not found";
                return json;
            }
            string name = vm["Name"].ToString();
            double balance = double.TryParse(vm["Balance"].ToString(), out _) ? Convert.ToDouble(vm["Balance"]) : 0;
            string balanceType = vm["Type"].ToString();
            bool isActive = Convert.ToBoolean(vm["IsActive"]);
            var exists = Exists(name);
            if (exists.StatusCode != 200)
            {
                return exists;
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    Account data = new Account();
                    data.GALId = GALId;
                    data.GALCode = GetGALCode(GALId).Value.ToString();
                    data.Code = GetMaxCode(GALId).Value.ToString();
                    data.Name = name;
                    data.TradeFirmId = 1;
                    data.IsActive = isActive;
                    data.UserId = usrMgr.GetUserId(User);

                    context.Accounts.Add(data);
                    context.SaveChanges();

                    AccountVM acc = new AccountVM();
                    acc.Id = data.Id;
                    acc.Name = data.Name;
                    acc.TransactionDate = new DateTime(1980, 01, 01, 00, 00, 00);
                    if (balanceType == "Debit")
                    {
                        acc.Balance = balance;
                    }
                    else
                    {
                        acc.Balance = balance * -1;
                    }
                    var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                    acc.TradeFirmId = tf.Id;
                    acc.TransactionStatusId = 1;
                    acc.UserId = usrMgr.GetUserId(User);

                    var ledger = Ledger(acc);
                    if (ledger.StatusCode != 200)
                    {
                        return ledger;
                    }

                    trans.Commit();
                    json.Value = "Success";
                    json.StatusCode = 200;
                    return json;
                }

            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
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
            if (string.IsNullOrEmpty(vm["Name"].ToString()))
            {
                json.Value = "Error! Name cannot be empty";
                return json;
            }
            if (OBDt == null)
            {
                json.Value = "Error! Opening stock Doc Type not found";
                return json;
            }
            if (CapitalAc == null)
            {
                json.Value = "Error! Capital Account not found";
                return json;
            }
            int id = Convert.ToInt32(vm["Id"]);
            string name = vm["Name"].ToString();
            string initialName = vm["InitialName"].ToString();
            double balance = double.TryParse(vm["Balance"].ToString(), out _) ? Convert.ToDouble(vm["Balance"]) : 0;
            string balanceType = vm["Type"].ToString();
            bool isActive = Convert.ToBoolean(vm["IsActive"]);
            if (initialName != name)
            {
                var exists = Exists(name);
                if (exists.StatusCode != 200)
                {
                    return exists;
                }
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    Account data = context.Accounts.Where(a => a.Id == id).FirstOrDefault();
                    data.Name = name;
                    data.IsActive = isActive;
                    data.TradeFirmId = 1;
                    data.UserId = usrMgr.GetUserId(User);

                    context.SaveChanges();

                    AccountVM acc = new AccountVM();
                    acc.Id = data.Id;
                    acc.Name = data.Name;
                    acc.TransactionDate = new DateTime(1980, 01, 01, 00, 00, 00);
                    if (balanceType == "Debit")
                    {
                        acc.Balance = balance;
                    }
                    else
                    {
                        acc.Balance = balance * -1;
                    }
                    var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
                    acc.TradeFirmId = tf.Id;
                    acc.TransactionStatusId = 2;
                    acc.UserId = usrMgr.GetUserId(User);

                    var remove = Remove(acc);
                    if (remove.StatusCode != 200)
                    {
                        return remove;
                    }

                    var ledger = Ledger(acc);
                    if (ledger.StatusCode != 200)
                    {
                        return ledger;
                    }

                    trans.Commit();
                    json.Value = "Success";
                    json.StatusCode = 200;
                    return json;
                }
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }

        public JsonResult Exists(string name)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.Accounts.Where(a => a.Name == name).FirstOrDefault();
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException.Message ?? "");
                return json;
            }
        }

        public JsonResult Ledger(AccountVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    LedgerDetail l = new LedgerDetail();
                    l.DocId = 0;
                    l.DocTypeId = OBDt.Id;
                    l.TransactionDate = vm.TransactionDate;
                    l.PostDate = DateTime.Now;
                    if (i == 0)
                    {
                        if (vm.Balance >= 0)
                        {
                            l.AccountId = vm.Id;
                            l.AgainstAccountId = CapitalAc.Id;
                            l.Narration = "Opening Balance of " + vm.Name;
                            l.NarrationDetailed = "Opening Balance of " + vm.Name;
                            l.Debit = vm.Balance;
                            l.Credit = 0;
                        }
                        else
                        {
                            l.AccountId = CapitalAc.Id;
                            l.AgainstAccountId = vm.Id;
                            l.Narration = "Opening Balance of " + vm.Name;
                            l.NarrationDetailed = "Opening Balance of " + vm.Name;
                            l.Debit = vm.Balance * -1;
                            l.Credit = 0;
                        }
                    }
                    if (i == 1)
                    {
                        if (vm.Balance >= 0)
                        {
                            l.AccountId = CapitalAc.Id;
                            l.AgainstAccountId = vm.Id;
                            l.Narration = "Opening Balance of " + vm.Name;
                            l.NarrationDetailed = "Opening Balance of " + vm.Name;
                            l.Debit = 0;
                            l.Credit = vm.Balance;
                        }
                        else
                        {
                            l.AccountId = vm.Id;
                            l.AgainstAccountId = CapitalAc.Id;
                            l.Narration = "Opening Balance of " + vm.Name;
                            l.NarrationDetailed = "Opening Balance of " + vm.Name;
                            l.Debit = 0;
                            l.Credit = vm.Balance * -1;
                        }
                    }
                    l.TradeFirmId = vm.TradeFirmId;
                    l.TransactionStatusId = vm.TransactionStatusId;
                    l.UserId = vm.UserId;

                    context.LedgerDetails.Add(l);
                    context.SaveChanges();
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }

        public JsonResult Remove(AccountVM vm)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                List<LedgerDetail> ld = context.LedgerDetails.Where(a => a.DocTypeId == OBDt.Id && (a.AccountId == vm.Id || a.AgainstAccountId == vm.Id)).ToList();
                if (ld.Count > 0)
                {
                    context.LedgerDetails.RemoveRange(ld);
                    context.SaveChanges();
                }

                json.Value = "Success";
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }

        [HttpPost, Authorize(Roles = "Admin,User")]
        public double PandL(IFormCollection vm)
        {
            if (!DateTime.TryParse(vm["FromDate"].ToString(), out _))
            {
                ViewBag.Error = "Error! Invalid From Date";
            }
            if (!DateTime.TryParse(vm["ToDate"].ToString(), out _))
            {
                ViewBag.Error = "Error! Invalid From Date";
            }
            DateTime fromDate = Convert.ToDateTime(vm["FromDate"]);
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 00, 00, 00);
            DateTime toDate = Convert.ToDateTime(vm["ToDate"]);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            ViewBag.DateRange = "From " + fromDate.ToString("dd/MM/yyyy") + " To " + toDate.ToString("dd/MM/yyyy");

            var tf = SysAcc.GetTF(context, usrMgr.GetUserId(User));
            List<RPTPLVM> data = new List<RPTPLVM>();
            double sales = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.Name == "Sale" && a.TransactionStatusId != 3
                                    && a.TransactionDate <= toDate).Sum(a => a.Credit - a.Debit);

            double salesReturn = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.Name == "Sale Return" && a.TransactionStatusId != 3
                                    && a.TransactionDate <= toDate).Sum(a => a.Debit - a.Credit);

            double cgs = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && (a.Account.Name == "CGS")
                                        && a.TransactionDate <= toDate && a.TransactionStatusId != 3).Sum(a => a.Debit - a.Credit);

            double adminExp = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.GAL.Name == "ADMINISTRATIVE EXPENSES"
                                        && a.TransactionDate <= toDate && a.TransactionStatusId != 3).Sum(a => a.Debit - a.Credit);

            double incomes = context.LedgerDetails.Where(a => a.TradeFirmId == tf.Id && a.Account.GAL.Name == "OTHER INCOME"
                            && a.TransactionDate <= toDate && a.TransactionStatusId != 3).Sum(a => a.Credit - a.Debit);

            double gp = sales - (salesReturn + cgs);
            double np = gp + incomes - adminExp;
            return Math.Round(np, 0);
        }
    }
}
