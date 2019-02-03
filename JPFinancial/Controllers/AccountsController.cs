using System;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Models;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    /// <summary>
    /// Handles all Account interactions with Views
    /// </summary>
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        // GET: Accounts
        public ActionResult Index()
        {
            try
            {
                AccountViewModel accountVM = new AccountViewModel();
                accountVM.EventArgument = EventArgumentEnum.Read;
                accountVM.EventCommand = EventCommandEnum.Get;
                accountVM.HandleRequest();
                //_dbEditor.UpdateRequiredBalanceForBills();
                //_dbEditor.UpdateRequiredBalanceSurplus();


                //TODO: Add ability to show X number of Accounts
                return View(accountVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
            }
        }

        //TODO: figure out a better way to update the ViewModel
        [HttpPost]
        public ActionResult Index(AccountViewModel accountVM)
        {
            try
            {
                accountVM.EventArgument = EventArgumentEnum.Read;
                accountVM.EventCommand = EventCommandEnum.Get;
                accountVM.HandleRequest();
                ModelState.Clear();

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
                if (id == null)
                {
                    Logger.Instance.Debug("Account ID is null");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AccountViewModel accountVM = new AccountViewModel();
                accountVM.EventArgument = EventArgumentEnum.Read;
                accountVM.EventCommand = EventCommandEnum.Details;
                accountVM.Entity.Account = _db.Accounts.Find(id);

                if (accountVM.Entity.Account != null) return View(accountVM);
                Logger.Instance.Debug("Returned Account is null");
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
            return View(new AccountViewModel());
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
                if (!ModelState.IsValid) return View(accountVM);


                _db.Accounts.Add(accountVM.Entity.Account);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new AccountViewModel());
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
                accountVM.Entity.Account = _db.Accounts.Find(id);
                if (accountVM.Entity.Account == null)
                {
                    return HttpNotFound();
                }
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
                accountVM.EventArgument = EventArgumentEnum.Update;
                accountVM.EventCommand = EventCommandEnum.Edit;
                if (accountVM.HandleRequest())
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
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Account account = _db.Accounts.Find(id);
                if (account == null)
                {
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
                Account account = _db.Accounts.Find(id);
                _db.Accounts.Remove(account);
                _db.SaveChanges();
                return RedirectToAction("Index");
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
                //AccountViewModel vm = new AccountViewModel();
                vm.EventArgument = EventArgumentEnum.Update;
                vm.EventCommand = EventCommandEnum.Update;
                vm.HandleRequest();

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
                vm.EventArgument = EventArgumentEnum.Update;
                vm.EventCommand = EventCommandEnum.Rebalance;
                vm.HandleRequest();


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
