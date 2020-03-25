using System;
using System.Collections.Generic;
using System.Web.Mvc;
using JPFData;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly AccountManager _accountManager = new AccountManager();

        // GET: Accounts
        public ActionResult Index()
        {
            try
            {
                var accountVM = new AccountViewModel();
                _accountManager.Update();
                _accountManager.Rebalance();
                accountVM.Accounts = _accountManager.GetAllAccounts();
                accountVM.Metrics = _accountManager.GetMetrics();
                accountVM.RebalanceReport = _accountManager.GetRebalancingAccountsReport();

                
                //TODO: Add ability to show X number of Accounts
                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error: " + e);
            }
        }

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
