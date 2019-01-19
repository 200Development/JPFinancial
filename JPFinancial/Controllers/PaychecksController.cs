using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Models;

namespace JPFinancial.Controllers
{
    public class PaychecksController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Paychecks
        public ActionResult Index()
        {
            return View(_db.Paychecks.ToList());
        }

        // GET: Paychecks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paycheck paycheck = _db.Paychecks.Find(id);
            if (paycheck == null)
            {
                return HttpNotFound();
            }
            return View(paycheck);
        }

        // GET: Paychecks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Paychecks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Regular,Employer,ElectronicsNontaxable,TravelBusinessExpenseNontaxable,HolidayPay,GrossPay,FederalTaxableGross,FederalWithholding,YTDFederalWithholding,FederalMedicaidWithholding,YTDFederalMedicaidWithholding,SocialSecurityWithholding,YTDSocialSecurityWithholding,StateTaxWithholding,YTDStateTaxWithholding,CityTaxWithholding,YTDCityTaxWithholding,IRA401KWithholding,YTDIRA401KWithholding,DependentCareFSAWithholding,YTDDependentCareFSAWithholding,HealthInsuranceWithholding,YTDHealthInsuranceWithholding,DentalInsuranceWithholding,YTDDentalInsuranceWithholding,TotalBeforeTaxDeductions,YTDTotalBeforeTaxDeductions,ChildSupportWithholding,YTDChildSupportWithholding,TotalAfterTaxDeductions,YTDTotalAfterTaxDeductions,TotalDeductions,YTDTotalDeductions,NetPay")] Paycheck paycheck)
        {
            if (!ModelState.IsValid) return View(paycheck);
            if (!AddIncomeToPool(paycheck)) return View(paycheck);
            if (!AddIncomeTransactionToDb(paycheck)) return View(paycheck);
            if (!TransferPaycheckContributions(paycheck, "create")) return View(paycheck);


            _db.Paychecks.Add(paycheck);
            _db.SaveChanges();


            return RedirectToAction("Index");
        }

        // GET: Paychecks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paycheck paycheck = _db.Paychecks.Find(id);
            if (paycheck == null)
            {
                return HttpNotFound();
            }
            return View(paycheck);
        }

        // POST: Paychecks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Regular,ElectronicsNontaxable,TravelBusinessExpenseNontaxable,HolidayPay,GrossPay,FederalTaxableGross,FederalWithholding,YTDFederalWithholding,FederalMedicaidWithholding,YTDFederalMedicaidWithholding,SocialSecurityWithholding,YTDSocialSecurityWithholding,StateTaxWithholding,YTDStateTaxWithholding,CityTaxWithholding,YTDCityTaxWithholding,FedWithholding,YTDFedWithholding,IRA401KWithholding,YTDIRA401KWithholding,DependentCareFSAWithholding,YTDDependentCareFSAWithholding,HealthInsuranceWithholding,YTDHealthInsuranceWithholding,DentalInsuranceWithholding,YTDDentalInsuranceWithholding,TotalBeforeTaxDeductions,YTDTotalBeforeTaxDeductions,ChildSupportWithholding,YTDChildSupportWithholding,TotalAfterTaxDeductions,YTDTotalAfterTaxDeductions,TotalDeductions,YTDTotalDeductions,NetPay,MaritalStatus,Allowances")] Paycheck paycheck)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(paycheck).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paycheck);
        }

        // GET: Paychecks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paycheck paycheck = _db.Paychecks.Find(id);
            if (paycheck == null)
            {
                return HttpNotFound();
            }
            return View(paycheck);
        }

        // POST: Paychecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Paycheck paycheck = _db.Paychecks.Find(id);
            _db.Paychecks.Remove(paycheck);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool AddIncomeToPool(Paycheck paycheck)
        {
            var poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
            if (poolAccount == null) return false;
            poolAccount.Balance += paycheck.NetPay;
            _db.Entry(poolAccount).State = EntityState.Modified;


            return true;
        }

        private bool AddIncomeTransactionToDb(Paycheck paycheck)
        {
            try
            {
                var newTransaction = new Transaction();
                newTransaction.Date = paycheck.Date;
                newTransaction.Payee = paycheck.Employer;
                newTransaction.Category = CategoriesEnum.NetSalary;
                newTransaction.Memo = string.Empty;
                newTransaction.Type = TransactionTypesEnum.Income;
                newTransaction.DebitAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                newTransaction.CreditAccount = null;
                newTransaction.Amount = paycheck.NetPay;
                _db.Entry(newTransaction).State = EntityState.Added;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool TransferPaycheckContributions(Paycheck paycheck, string type)
        {
            if (type != "create") return true;

            var accounts = _db.Accounts.Where(a => a.PaycheckContribution > 0).ToList();
            var poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
            if (poolAccount == null) throw new InvalidOperationException("No Pool Account is Assigned");

            foreach (var account in accounts)
            {
                decimal paycheckContribution;
                if (account.PaycheckContribution != null)
                    paycheckContribution = (decimal)account.PaycheckContribution;
                else
                    continue; //Don't enter the transaction if there's no Paycheck Contribution set for the Account


                var newTransaction = new Transaction();
                newTransaction.Date = paycheck.Date;
                newTransaction.Payee = $"Transfer to {account.Name}";
                newTransaction.Category = CategoriesEnum.Transfer;
                newTransaction.Memo = string.Empty;
                newTransaction.Type = TransactionTypesEnum.Transfer;
                newTransaction.DebitAccount = account;
                newTransaction.CreditAccount = poolAccount;
                newTransaction.Amount = paycheckContribution;


                //Update Account Balances
                account.Balance += paycheckContribution;
                poolAccount.Balance -= paycheckContribution;

                _db.Transactions.Add(newTransaction);
                _db.Entry(account).State = EntityState.Modified;
                _db.Entry(poolAccount).State = EntityState.Modified;
            }

            return true;
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
