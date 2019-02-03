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
            AccountViewModel accountVM = new AccountViewModel();
            accountVM.EventArgument = EventArgumentEnum.Read;
            accountVM.EventCommand = EventCommandEnum.Get;
            accountVM.HandleRequest();
            //_dbEditor.UpdateRequiredBalanceForBills();
            //_dbEditor.UpdateRequiredBalanceSurplus();


            //TODO: Add ability to show X number of Accounts
            return View(accountVM);
        }

        //TODO: figure out a better way to update the ViewModel
        [HttpPost]
        public ActionResult Index(AccountViewModel accountVM)
        {
            accountVM.EventArgument = EventArgumentEnum.Read;
            accountVM.EventCommand = EventCommandEnum.Get;
            accountVM.HandleRequest();
            ModelState.Clear();

            return View(accountVM);
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
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
            if (accountVM.Entity.Account == null)
            {
                Logger.Instance.Debug("Returned Account is null");
                return HttpNotFound();
            }
            return View(accountVM);
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
            if (!ModelState.IsValid) return View(accountVM);


            _db.Accounts.Add(accountVM.Entity.Account);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountViewModel accountVM)
        {
            if (!ModelState.IsValid) return View(accountVM);
            accountVM.EventArgument = EventArgumentEnum.Update;
            accountVM.EventCommand = EventCommandEnum.Edit;
            if (accountVM.HandleRequest())
                return RedirectToAction("Index");


            return View(accountVM);
        }

        //GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = _db.Accounts.Find(id);
            _db.Accounts.Remove(account);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [ActionName("Update")]
        public ActionResult Update(AccountViewModel vm)
        {
            //AccountViewModel vm = new AccountViewModel();
            vm.EventArgument = EventArgumentEnum.Update;
            vm.EventCommand = EventCommandEnum.Update;
            vm.HandleRequest();
            //vm.Entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(vm.Entity);

            return RedirectToAction("Index", vm);
        }

        [ActionName("Rebalance")]
        public ActionResult Rebalance(AccountViewModel vm)
        {
            vm.EventArgument = EventArgumentEnum.Update;
            vm.EventCommand = EventCommandEnum.Rebalance;
            vm.HandleRequest();


            return RedirectToAction("Index", vm);
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
