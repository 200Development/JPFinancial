using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JPFData;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;
using PagedList;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly AccountManager _accountManager = new AccountManager();

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                // Prevents page from breaking due to error updating or rebalancing
                try
                {
                    _accountManager.Update();
                    _accountManager.Rebalance();
                }
                catch (Exception e)
                {
                    Logger.Instance.Error(e);
                }
                
                var accountVM = GetAccountVM(1);
              
             
                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error: " + e);
            }
        }

        [HttpPost]
        public ActionResult Create(AccountViewModel accountVM)
        {
            try
            {

                if (!_accountManager.Create(accountVM.Account)) return View("Error");


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        [HttpPost]
        public JsonResult EditAccount(Account account)
        {
            try
            {
                return Json(_accountManager.Edit(account) ? "Success" : "Failed");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        [HttpPost]
        public JsonResult DeleteAccount(Account account)
        {
            try
            {
                //TODO: Add verification and notification of success 
                return account.Id > 0 ? Json(_accountManager.Delete(account.Id) ? "Success" : "Failed") : Json("ID is invalid");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error: " + e);
            }
        }

        [HttpPost]
        public JsonResult UpdateAccounts(List<Account> accounts)
        {
            try
            {
                return accounts.Count > 0
                    ? Json(_accountManager.UpdateAccountsFromDashboard(accounts) ? "Success" : "Failed") : Json("No Accounts received");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error: " + e);
            }
        }

        [HttpGet]
        public ActionResult PageAccounts(int page = 1)
        {
            try
            {

                var accountVM = GetAccountVM(page);               
           

                return PartialView("_AccountsTable", accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        private AccountViewModel GetAccountVM(int page)
        {
            var pageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["defaultPageSize"]);

            var accountVM = new AccountViewModel();
            accountVM.Accounts = _accountManager.GetAllAccounts().OrderByDescending(a => a.Balance).ThenBy(a => a.Name).ToList();
            accountVM.PagedAccounts = accountVM.Accounts.ToPagedList(page, pageSize);


            return accountVM;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
