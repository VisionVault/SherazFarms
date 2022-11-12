using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CustomerController : Controller
    {
        private readonly TMSContext context;
        private readonly UserManager<User> usrMgr;
        private readonly DocType OBDt;
        private readonly GAL CusGAL;
        private readonly GAL SupGAL;
        private readonly Account CapitalAc;

        public CustomerController(TMSContext context, UserManager<User> usrMgr)
        {
            this.context = context;
            this.usrMgr = usrMgr;
            OBDt = context.DocTypes.Where(a => a.Name == "OB").FirstOrDefault();
            CusGAL = context.GALs.Where(a => a.Name == "TRADE DEBTS").FirstOrDefault();
            SupGAL = context.GALs.Where(a => a.Name == "TRADE AND OTHER PAYABLES").FirstOrDefault();
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }
        public JsonResult Search(string param)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = (from cc in context.AccountDetails
                            select new
                            {
                                Id = cc.AccountId,
                                cc.Account.Name,
                                Contact = cc.Contact,
                            }).Take(50).ToList();
                if (!string.IsNullOrEmpty(param))
                {
                    code = (from cc in context.AccountDetails
                            select new
                            {
                                Id = cc.AccountId,
                                cc.Account.Name,
                                Contact = cc.Contact,
                            }).Where(a => (a.Name + a.Contact).ToLower().Contains(param.ToLower())).ToList();
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
        public JsonResult GetAll()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var code = context.AccountDetails.Where(a => a.Account.GALId == CusGAL.Id)
                    .Select(a => new
                    {
                        Id = a.AccountId,
                        Code = a.Account.GALCode + a.Account.Code,
                        a.Account.Name,
                        a.Contact,
                        City = a.City.Name,
                        a.Account.IsActive,
                    }).OrderBy(a => Convert.ToInt32(a.Code)).ToList();
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
                var code = (from a in context.AccountDetails.Where(a => a.AccountId == id)

                            let ob = context.LedgerDetails.Where(b => b.AccountId == a.AccountId && b.DocTypeId == OBDt.Id && b.TransactionStatusId != 3).FirstOrDefault()
                            select new
                            {
                                Id = a.AccountId,
                                a.Account.GALId,
                                a.Account.GALCode,
                                GAL = a.Account.GAL.Name,
                                a.Account.Code,
                                a.Account.Name,
                                InitialName = a.Account.Name,
                                a.Account.IsActive,
                                a.Contact,
                                InitialContact = a.Contact,
                                a.Address,
                                a.CityId,
                                City = a.City.Name,
                                OpBalance = ob.Debit - ob.Credit
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
            if (CusGAL == null)
            {
                json.Value = "Error! Customer GAL not found";
                return json;
            }
            if (GetMaxCode(CusGAL.Id).StatusCode != 200)
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
            string contact = vm["Contact"].ToString();
            string address = vm["Address"].ToString();
            int? cityId = null;
            if (int.TryParse(vm["CityId"].ToString(), out _))
            {
                cityId = Convert.ToInt32(vm["CityId"]);
            }
            double balance = double.TryParse(vm["Balance"].ToString(), out _) ? Convert.ToDouble(vm["Balance"]) : 0;
            string balanceType = vm["Type"].ToString();
            bool isActive = Convert.ToBoolean(vm["IsActive"]);
            var exists = Exists(name, contact);
            if (exists.StatusCode != 200)
            {
                return exists;
            }
            try
            {
                using (IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    Account data = new Account();
                    data.GALId = CusGAL.Id;
                    data.GALCode = CusGAL.CTLCode + CusGAL.Code;
                    data.Code = GetMaxCode(CusGAL.Id).Value.ToString();
                    data.Name = name;
                    data.TradeFirmId = 1;
                    data.IsActive = isActive;
                    data.UserId = usrMgr.GetUserId(User);

                    context.Accounts.Add(data);
                    context.SaveChanges();

                    AccountDetail cd = new AccountDetail();
                    cd.AccountId = data.Id;
                    cd.Contact = contact;
                    cd.Address = address;
                    cd.CityId = cityId;

                    context.AccountDetails.Add(cd);
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
                    acc.TradeFirmId = 1;
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
            string initialContact = vm["InitialContact"].ToString();
            string contact = vm["Contact"].ToString();
            string address = vm["Address"].ToString();
            int? cityId = null;
            if (int.TryParse(vm["CityId"].ToString(), out _))
            {
                cityId = Convert.ToInt32(vm["CityId"]);
            }
            double balance = double.TryParse(vm["Balance"].ToString(), out _) ? Convert.ToDouble(vm["Balance"]) : 0;
            string balanceType = vm["Type"].ToString();
            bool isActive = Convert.ToBoolean(vm["IsActive"]);
            if (initialName != name && initialContact != contact)
            {
                var exists = Exists(name, contact);
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
                    data.TradeFirmId = 1;
                    data.IsActive = isActive;
                    data.UserId = usrMgr.GetUserId(User);

                    context.SaveChanges();

                    AccountDetail cd = context.AccountDetails.Where(a => a.AccountId == id).FirstOrDefault();
                    cd.Contact = contact;
                    cd.Address = address;
                    cd.CityId = cityId;

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
                    acc.TradeFirmId = 1;
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
                return json;
            }
        }

        public JsonResult Exists(string name, string contact)
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.AccountDetails.Where(a => a.Account.Name == name && a.Contact == contact).FirstOrDefault();
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
                json.Value = "Error! " + ex.Message + " " + (ex.InnerException == null ? "" : ex.InnerException.Message);
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
    }
}
