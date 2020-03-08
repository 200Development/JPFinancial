using System;
using System.Net;
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

        //TODO: figure out a better way to update the ViewModel
        [HttpPost]
        public ActionResult Index(AccountViewModel accountVM)
        {
            try
            {
                Logger.Instance.DataFlow($"Index (w/ VM)");
                var accounts = _accountManager.GetAllAccounts();
                _accountManager.Update(accounts);
                _accountManager.Rebalance(accounts);
                accountVM.Accounts = _accountManager.GetAllAccounts();
                accountVM.Metrics = _accountManager.GetMetrics();
                accountVM.RebalanceReport = _calc.GetRebalancingAccountsReport();


                ModelState.Clear();
                Logger.Instance.DataFlow($"ModelState cleared");


                Logger.Instance.DataFlow($"AccountViewModel returned to View");
                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
            }
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                Logger.Instance.DataFlow($"Details");
                if (id == null) 
                {
                    Logger.Instance.Debug("Account ID is null");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                AccountViewModel accountVM = new AccountViewModel();
                accountVM.Account = _accountManager.GetAccount(id);


                if (accountVM.Account != null)
                {
                    Logger.Instance.DataFlow($"AccountViewModel returned to View");
                    return View(accountVM);
                }


                Logger.Instance.Debug("Returned Account is null - (error)");
                return HttpNotFound();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
            }
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            try
            {
                AccountViewModel accountVM = new AccountViewModel();


                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
            }
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountViewModel accountVM)
        {
            try
            {
                if (!ModelState.IsValid) return View("Error");
                if (!_accountManager.Create(accountVM.Account)) return View("Error");


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                AccountViewModel accountVM = new AccountViewModel();
                accountVM.Account = _accountManager.GetAccount(id);


                if (accountVM.Account == null)
                {
                    Logger.Instance.Debug("Returned Account is null - (error)");
                    return HttpNotFound();
                }

                accountVM.Accounts = _accountManager.GetAllAccounts();


                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
            }
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountViewModel accountVM)
        {
            try
            {
                if (!ModelState.IsValid) return View(accountVM);
                if (_accountManager.Edit(accountVM.Account))
                    return RedirectToAction("Index");


                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
            }
        }

        //GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                Account account = _accountManager.GetAccount(id);
                if (account == null)
                {
                    Logger.Instance.Debug("Returned Account is null - (error)");
                    return HttpNotFound();
                }


                return View(account);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new Account());
            }
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (_accountManager.Delete(id))
                    return RedirectToAction("Index");

                // Send Account back to Delete View if delete failed
                Account account = _accountManager.GetAccount(id);
                if (account == null)
                {
                    Logger.Instance.Debug("Returned Account is null - (error)");
                    return HttpNotFound();
                }


                return View("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index");
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
