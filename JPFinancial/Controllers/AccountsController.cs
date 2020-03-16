using System;
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
                var accounts = _accountManager.GetAllAccounts();
                _accountManager.Update(accounts);
                //  _accountManager.Rebalance(accounts);
                accountVM.Accounts = _accountManager.GetAllAccounts();
                accountVM.Metrics = _accountManager.GetMetrics();
                accountVM.RebalanceReport = _accountManager.GetRebalancingAccountsReport();

                
                //TODO: Add ability to show X number of Accounts
                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
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
                return Json("Error");
            }
        }

        [ActionName("Update")]
        public ActionResult Update(AccountViewModel vm)
        {
            try
            {
                var accounts = _accountManager.GetAllAccounts();
                _accountManager.Update(accounts);
                vm.Accounts = _accountManager.GetAllAccounts();
                vm.Metrics = _accountManager.GetMetrics();

                return RedirectToAction("Index", vm);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index", new AccountViewModel());
            }
        }

        [ActionName("Rebalance")]
        public ActionResult Rebalance(AccountViewModel vm)
        {
            try
            {
                var accounts = _accountManager.GetAllAccounts();
                _accountManager.Rebalance(accounts);
                vm.Accounts = _accountManager.GetAllAccounts();
                vm.Metrics = _accountManager.GetMetrics();

                return RedirectToAction("Index", vm);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index", new AccountViewModel());
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
