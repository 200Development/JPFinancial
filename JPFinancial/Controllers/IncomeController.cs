using System;
using System.Net;
using System.Web.Mvc;
//using JPFData;
using JPFData.Enumerations;
//using JPFData.Models;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    public class IncomeController : Controller
    {
        //private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Salary
        public ActionResult Index()
        {
            IncomeViewModel incomeVM = new IncomeViewModel();
            incomeVM.EventArgument = EventArgumentEnum.Read;
            incomeVM.EventCommand = EventCommandEnum.Get;
            incomeVM.HandleRequest();
            return View(incomeVM);
        }

        // GET: Salary/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IncomeViewModel incomeVM = new IncomeViewModel();
            incomeVM.EventArgument = EventArgumentEnum.Read;
            incomeVM.EventCommand = EventCommandEnum.Details;
            incomeVM.HandleRequest();

            //Salary salary = _db.Salaries.Find(id);
            //if (salary == null)
            //{
            //    return HttpNotFound();
            //}
            return View(incomeVM);
        }
         
        // GET: Salary/Create
        public ActionResult Create()
        {
            return View(new IncomeViewModel());
        }

        // POST: Salary/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IncomeViewModel incomeVM)
        {
            if (!ModelState.IsValid) return View(incomeVM);
            incomeVM.EventArgument = EventArgumentEnum.Create;
            incomeVM.Entity.Paycheck.Date = Convert.ToDateTime(incomeVM.Date);
            incomeVM.Entity.AutoTransferPaycheckContributions = incomeVM.AutoTransferPaycheckContributions;
            if(incomeVM.HandleRequest())
                return RedirectToAction("Index");

            return View(incomeVM);


            //var newSalary = new Salary()
            //{
            //    NetIncome = salary.NetIncome,
            //    PayFrequency = salary.PayFrequency,
            //    Payee = salary.Payee,
            //    GrossPay = salary.GrossPay,
            //    PayTypesEnum = salary.PayTypesEnum
            //};

            //_db.Salaries.Add(newSalary);
            //_db.SaveChanges();


        }

        // GET: Salary/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            IncomeViewModel incomeVM = new IncomeViewModel();
            incomeVM.EventArgument = EventArgumentEnum.Update;
            incomeVM.EventCommand = EventCommandEnum.Get;
            incomeVM.Entity.Paycheck.Id = (int) id;
            incomeVM.HandleRequest();
            if (incomeVM.Entity.Paycheck == null)
            {
                return HttpNotFound();
            }


            return View(incomeVM);
        }

        // POST: Salary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IncomeViewModel incomeVM)
        {
            if (!ModelState.IsValid) return View(incomeVM);
            incomeVM.EventArgument = EventArgumentEnum.Update;
            incomeVM.EventCommand = EventCommandEnum.Edit;
            if(incomeVM.HandleRequest())
                return RedirectToAction("Index");


            return View(incomeVM);
            //_db.Entry(salary).State = EntityState.Modified;
            //_db.SaveChanges();
            
        }

        // GET: Salary/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IncomeViewModel incomeVM = new IncomeViewModel();
            incomeVM.EventArgument = EventArgumentEnum.Delete;
            incomeVM.EventCommand = EventCommandEnum.Get;
            incomeVM.Entity.Paycheck.Id = (int) id;
            if (incomeVM.HandleRequest())
                return View(incomeVM);


            return HttpNotFound();

            //if (salary == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(salary);
        }

        // POST: Salary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IncomeViewModel incomeVM = new IncomeViewModel();
            incomeVM.EventArgument = EventArgumentEnum.Delete;
            incomeVM.EventCommand = EventCommandEnum.Delete;
            incomeVM.Entity.Paycheck.Id = id;
            incomeVM.HandleRequest();
            //Salary salary = _db.Salaries.Find(id);
            //_db.Salaries.Remove(salary);
            //_db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
