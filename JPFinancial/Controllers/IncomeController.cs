using System;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        // GET: Salary
        public ActionResult Index()
        {
            try
            {
                IncomeViewModel incomeVM = new IncomeViewModel();
                incomeVM.EventArgument = EventArgumentEnum.Read;
                incomeVM.EventCommand = EventCommandEnum.Get;
                incomeVM.HandleRequest();
                return View(incomeVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
        }

        // GET: Salary/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                IncomeViewModel incomeVM = new IncomeViewModel();
                incomeVM.EventArgument = EventArgumentEnum.Read;
                incomeVM.EventCommand = EventCommandEnum.Details;
                incomeVM.HandleRequest();
                return View(incomeVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
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
            try
            {
                if (!ModelState.IsValid) return View(incomeVM);
                incomeVM.EventArgument = EventArgumentEnum.Create;
                incomeVM.Entity.Paycheck.Date = Convert.ToDateTime(incomeVM.Date);
                incomeVM.Entity.AutoTransferPaycheckContributions = incomeVM.AutoTransferPaycheckContributions;
                if(incomeVM.HandleRequest())
                    return RedirectToAction("Index");

                return View(incomeVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
        }

        // GET: Salary/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
        }

        // POST: Salary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IncomeViewModel incomeVM)
        {
            try
            {
                if (!ModelState.IsValid) return View(incomeVM);
                incomeVM.EventArgument = EventArgumentEnum.Update;
                incomeVM.EventCommand = EventCommandEnum.Edit;
                if(incomeVM.HandleRequest())
                    return RedirectToAction("Index");


                return View(incomeVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
        }

        // GET: Salary/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                IncomeViewModel incomeVM = new IncomeViewModel();
                incomeVM.EventArgument = EventArgumentEnum.Delete;
                incomeVM.EventCommand = EventCommandEnum.Get;
                incomeVM.Entity.Paycheck.Id = (int) id;
                if (incomeVM.HandleRequest())
                    return View(incomeVM);


                return HttpNotFound();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
        }

        // POST: Salary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                IncomeViewModel incomeVM = new IncomeViewModel();
                incomeVM.EventArgument = EventArgumentEnum.Delete;
                incomeVM.EventCommand = EventCommandEnum.Delete;
                incomeVM.Entity.Paycheck.Id = id;
                incomeVM.HandleRequest();


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new IncomeViewModel());
            }
        }
    }
}
