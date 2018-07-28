using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Models;
using JPFinancial.Models;
using JPFinancial.ViewModels;

namespace JPFinancial.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();

        // GET: Loans
        public ActionResult Index()
        {
            var months = 12;
            var loansVM = new List<LoanViewModel>();
            var financialsPerMonth = new List<Dictionary<DateTime, LoanViewModel>>();

            foreach (var loan in _db.Loans.ToList())
            {
                var newLoanVM = new LoanViewModel();
                newLoanVM.Id = loan.Id;
                newLoanVM.Name = loan.Name;
                newLoanVM.LoanOriginationDate = loan.LoanOriginationDate;
                newLoanVM.OriginalLoanAmount = loan.OriginalLoanAmount;
                newLoanVM.OutstandingBalance = loan.OutstandingBalance;
                newLoanVM.Payment = loan.Payment;
                newLoanVM.APR = loan.APR;
                newLoanVM.DailyInterestCost = _calculations.CalculateDailyInterestCost(loan);

                loansVM.Add(newLoanVM);
            }

           return View(loansVM);
        }

        // GET: Loans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = _db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,LoanOriginationDate,Term,DueDayOfMonth,TermClassification,OriginalLoanAmount,OutstandingBalance,APR,Payment,PaymentFrequency")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                loan.LoanOriginationDate = Convert.ToDateTime(loan.LoanOriginationDate);
                loan.NextDueDate = DateTime.MinValue;
                _db.Loans.Add(loan);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loan);
        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = _db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,LoanOriginationDate,Term,TermClassification,DueDayOfMonth,OriginalLoanAmount,PrincipalBalance,OutstandingBalance,APR,AccruedInterest,CapitalizedInterest,CompoundFrequency,Payment,Fees,Payments,PaymentFrequency")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(loan).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loan);
        }

        // GET: Loans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = _db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Loan loan = _db.Loans.Find(id);
            _db.Loans.Remove(loan);
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
