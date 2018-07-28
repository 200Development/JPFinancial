using System.Collections.Generic;
using JPFinancial.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Models;
using JPFinancial.ViewModels;

namespace JPFinancial.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();

        // GET: Accounts
        public ActionResult Index()
        {
            _calculations.GetRequiredAcctSavings();
            _calculations.GetReqBalanceSurplus();
            var accounts = _db.Accounts.ToList();
            var viewModel = new List<AccountViewModel>();

            foreach (var account in accounts)
            {
                var vm = new AccountViewModel();
                vm.Name = account.Name;
                vm.Balance = account.Balance;
                vm.BalanceSurplus = account.BalanceSurplus;
                vm.Id = account.Id;
                vm.PaycheckContribution = account.PaycheckContribution;
                vm.RequiredSavings = account.RequiredSavings;
                vm.BalanceFontColor = account.Balance < 0.0m ? "red" : "green";
                vm.SurplusFontColor = account.BalanceSurplus < 0.0m ? "red" : "green";

                viewModel.Add(vm);
            }

            return View(viewModel);
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
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Balance,PaycheckContribution,RequiredSavings")] Account account)
        {
            if (ModelState.IsValid)
            {
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
        public ActionResult Edit([Bind(Include = "Id,Name,Balance,PaycheckContribution,RequiredSavings")] Account account)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
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
