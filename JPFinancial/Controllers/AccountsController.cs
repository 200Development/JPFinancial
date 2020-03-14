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
        private readonly  AccountManager _accountManager = new AccountManager();
        private readonly Calculations _calc = new Calculations();

        // GET: Accounts
        public ActionResult Index()
        {
            try
            {
                AccountViewModel accountVM = new AccountViewModel();
                var accounts = _accountManager.GetAllAccounts();
                _accountManager.Update(accounts);
              //  _accountManager.Rebalance(accounts);
                accountVM.Accounts = _accountManager.GetAllAccounts();
                accountVM.Metrics = _accountManager.GetMetrics();
                accountVM.RebalanceReport = _calc.GetRebalancingAccountsReport();



                //TODO: Add ability to show X number of Accounts
                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(AccountViewModel accountVM, string submitButton)
        {
            try
            {
                if (!ModelState.IsValid) return View("Error");
                switch (submitButton)
                {
                    case "Add":
                        Create(accountVM);
                        break;
                    case "Edit":
                        Edit(accountVM);
                        break;
                    case "Delete":
                        var id = accountVM.Account.Id;

                        if (id > 0)
                            Delete(id);
                        else
                            return View("Error");
                        break;
                    default:
                        throw new NotImplementedException($"Form submit event {submitButton} is not currently handled");
                }


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

        private ActionResult Create(AccountViewModel accountVM)
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
        
        private ActionResult Edit(AccountViewModel accountVM)
        {
            try
            {
                if (!ModelState.IsValid) return View(accountVM);
                if (_accountManager.Edit(accountVM.Account))
                    return RedirectToAction("Index");


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }
        
        private ActionResult Delete(int id)
        {
            try
            {
                if (_accountManager.Delete(id))
                    return RedirectToAction("Index");


                return View("Error");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
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

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
