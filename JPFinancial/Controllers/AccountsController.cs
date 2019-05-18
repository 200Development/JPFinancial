using System;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        // GET: Accounts
        public ActionResult Index()
        {
            try
            {
                Logger.Instance.DataFlow($"Index");
                AccountViewModel accountVM = new AccountViewModel();
                accountVM.EventArgument = EventArgumentEnum.Read;
                accountVM.EventCommand = EventCommandEnum.Get;
                accountVM.HandleRequest();

                Logger.Instance.DataFlow($"AccountViewModel returned to View");
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
                Logger.Instance.DataFlow($"Index (w/ VM)");
                accountVM.EventArgument = EventArgumentEnum.Read;
                accountVM.EventCommand = EventCommandEnum.Get;
                accountVM.HandleRequest();
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
                accountVM.EventArgument = EventArgumentEnum.Read;
                accountVM.EventCommand = EventCommandEnum.Details;
                accountVM.Entity.Account = _db.Accounts.Find(id);
                Logger.Instance.DataFlow($"Pull Account with ID {id} from DB and set to AccountViewModel.Entity.Account");


                if (accountVM.Entity.Account != null)
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
            var accountVM = new AccountViewModel();
            accountVM.Entity.Account.UserId = Global.Instance.User.Id;
            return View(accountVM);
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
                Logger.Instance.DataFlow($"Create");
                if (!ModelState.IsValid) return View(accountVM);

                accountVM.EventArgument = EventArgumentEnum.Create;
                if(!accountVM.HandleRequest()) return View(accountVM);

                Logger.Instance.DataFlow($"Redirect to Account.Index View");
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
                Logger.Instance.DataFlow($"Edit - load");
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AccountViewModel accountVM = new AccountViewModel();
                accountVM.Entity.Account = _db.Accounts.Find(id);
                Logger.Instance.DataFlow($"Pull Account with ID {id} from DB and set to AccountViewModel.Entity.Account");

                if (accountVM.Entity.Account == null)
                {
                    Logger.Instance.Debug("Returned Account is null - (error)");
                    return HttpNotFound();
                }


                Logger.Instance.DataFlow($"AccountViewModel returned to View");
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
                Logger.Instance.DataFlow($"Edit - save");
                if (!ModelState.IsValid) return View(accountVM);
                accountVM.EventArgument = EventArgumentEnum.Update;
                accountVM.EventCommand = EventCommandEnum.Edit;
                if (accountVM.HandleRequest())
                {
                    Logger.Instance.DataFlow($"Redirect to Account.Index View");
                    return RedirectToAction("Index");
                }


                Logger.Instance.DataFlow($"AccountViewModel returned to View - (error)");
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
                Logger.Instance.DataFlow($"Delete - confirm");
                if (id == null)
                {
                    Logger.Instance.Debug("Account Id is null - (error)");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Account account = _db.Accounts.Find(id);
                Logger.Instance.DataFlow($"Pull Account with ID {id} from DB and set to AccountViewModel.Entity.Account");
                if (account == null)
                {
                    Logger.Instance.Debug("Returned Account is null - (error)");
                    return HttpNotFound();
                }


                Logger.Instance.DataFlow($"Account returned to View");
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
                Logger.Instance.DataFlow($"Delete - remove account");
                Account account = _db.Accounts.Find(id);
                Logger.Instance.DataFlow($"Pull Account with ID {id} from DB and set to AccountViewModel.Entity.Account");
                _db.Accounts.Remove(account);
                Logger.Instance.DataFlow($"Remove Account from data context");
                _db.SaveChanges();
                Logger.Instance.DataFlow($"Changes saved to DB");


                Logger.Instance.DataFlow($"Redirect to Account.Index View");
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
                Logger.Instance.DataFlow($"Update");
                vm.EventArgument = EventArgumentEnum.Update;
                vm.EventCommand = EventCommandEnum.Update;
                vm.HandleRequest();


                Logger.Instance.DataFlow($"Redirect to Account.Index View with passed AccountViewModel");
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
                Logger.Instance.DataFlow($"Rebalance");
                vm.EventArgument = EventArgumentEnum.Update;
                vm.EventCommand = EventCommandEnum.Rebalance;
                vm.HandleRequest();


                Logger.Instance.DataFlow($"Redirect to Account.Index View with passed AccountViewModel");
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
