using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.DTO;
using JPFData.Models;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly DatabaseEditor _dbEditor = new DatabaseEditor();

        // GET: Accounts
        public ActionResult Index()
        {
            AccountViewModel vm = new AccountViewModel();
            vm.HandleRequest();
            vm.EventArgument = "Get";
            _dbEditor.UpdateRequiredBalance();
            _dbEditor.UpdateRequiredBalanceSurplus();


            //TODO: Add ability to show X number of Accounts
            return View(vm);
        }

        //TODO: figure out a better way to update the ViewModel
        [HttpPost]
        public ActionResult Index(AccountViewModel model)
        {
            AccountViewModel vm = new AccountViewModel();
            vm.HandleRequest();
            vm.Entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(vm.Entity);
            ModelState.Clear();

            return View(vm);
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
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

        // GET: Accounts/Create
        public ActionResult Create()
        {
            return View(new Account());
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CurrentBalance,PaycheckContribution,RequiredSavings")] Account account)
        {
            if (ModelState.IsValid)
            {
                //Set RequiredSavings to $0.00 if nothing was entered by user
                if (account.RequiredSavings == null)
                    account.RequiredSavings = decimal.Zero;

                _db.Accounts.Add(account);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CurrentBalance,PaycheckContribution,RequiredSavings")] Account account)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        [HttpPost]
        public ActionResult Rebalance(AccountViewModel dto)
        {
            AccountViewModel vm = new AccountViewModel();
            vm.HandleRequest();
            vm.Entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(vm.Entity);

            return RedirectToAction("Index", vm);
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
