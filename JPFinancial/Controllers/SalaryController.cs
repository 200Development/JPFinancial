using System.Collections.Generic;
using JPFinancial.Models;
using JPFinancial.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFinancial.Models.Enumerations;

namespace JPFinancial.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Salary
        public ActionResult Index()
        {
            return View(_db.Salaries.ToList());
        }

        // GET: Salary/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = _db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            return View(salary);
        }

        // GET: Salary/Create
        public ActionResult Create()
        {
            CreateSalaryViewModel viewModel = new CreateSalaryViewModel();
            viewModel.Paydays = new DaysOfMonthEnum();

            return View(viewModel);
        }

        // POST: Salary/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Payee,PayType,PayFrequency,NetIncome,GrossPay")] CreateSalaryViewModel salary)
        {
            if (ModelState.IsValid)
            {
                var company = _db.Companies.FirstOrDefault(c => c.Name == salary.Payee);
                var newSalary = new Salary()
                {
                    NetIncome = salary.NetIncome,
                    PayFrequency = salary.PayFrequency,
                    Payee = salary.Payee,
                    GrossPay = salary.GrossPay,
                    PayTypesEnum = salary.PayTypesEnum
                };

                _db.Salaries.Add(newSalary);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(salary);
        }

        // GET: Salary/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = _db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            return View(salary);
        }

        // POST: Salary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PayType,PayFrequency,GrossPay")] Salary salary)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(salary).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salary);
        }

        // GET: Salary/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = _db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            return View(salary);
        }

        // POST: Salary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Salary salary = _db.Salaries.Find(id);
            _db.Salaries.Remove(salary);
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
