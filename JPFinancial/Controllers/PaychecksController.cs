using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Models;

namespace JPFinancial.Controllers
{
    public class PaychecksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Paychecks
        public ActionResult Index()
        {
            return View(db.Paychecks.ToList());
        }

        // GET: Paychecks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paycheck paycheck = db.Paychecks.Find(id);
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
            if (ModelState.IsValid)
            {
                db.Paychecks.Add(paycheck);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paycheck);
        }

        // GET: Paychecks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paycheck paycheck = db.Paychecks.Find(id);
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
                db.Entry(paycheck).State = EntityState.Modified;
                db.SaveChanges();
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
            Paycheck paycheck = db.Paychecks.Find(id);
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
            Paycheck paycheck = db.Paychecks.Find(id);
            db.Paychecks.Remove(paycheck);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
