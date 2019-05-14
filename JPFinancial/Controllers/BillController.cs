using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Bills
        public ActionResult Index()
        {
            return View(_db.Bills.ToList());
        }

        // GET: Bills/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Bill bill = _db.Bills.Find(id);
                if (bill == null)
                {
                    return HttpNotFound();
                }
                return View(bill);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new Bill());
            }
        }

        // GET: Bills/Create
        public ActionResult Create()
        {
            try
            {
                var viewModel = new BillViewModel
                {
                    Accounts = _db.Accounts.ToList()
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new BillViewModel());
            }
        }

        // POST: Bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BillViewModel billVM)
        {
            try
            {
                Logger.Instance.DataFlow($"Create");
                if (!ModelState.IsValid) return View(billVM);

                billVM.EventArgument = EventArgumentEnum.Create;
                if (!billVM.HandleRequest()) return View(billVM);

                Logger.Instance.DataFlow($"Redirect to Bill.Index View");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index");
            }
        }

        //public static IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Account> accounts, int selectedId)
        //{
        //    try
        //    {
        //        return accounts.OrderBy(account => account.Name).Select(account => new SelectListItem
        //        {
        //            Selected = (account.Id == selectedId),
        //            Text = account.Name,
        //            Value = account.Id.ToString()
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Instance.Error(e);
        //        return null;
        //    }
        //}

        // GET: Bills/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Bill bill = _db.Bills.Find(id);

                if (bill == null)
                {
                    return HttpNotFound();
                }
                BillViewModel viewModel = new BillViewModel();
                var account = _db.Accounts.Single(a => a.Id == bill.AccountId);
                //var accountId = account.Id;

                viewModel.Name = bill.Name;
                viewModel.AccountId = bill.AccountId;
                viewModel.Account = account;
                viewModel.AmountDue = bill.AmountDue;
                viewModel.DueDate = bill.DueDate;
                viewModel.Id = bill.Id;
                //viewModel.IsMandatory = bill.IsMandatory;
                viewModel.PaymentFrequency = bill.PaymentFrequency;
                viewModel.Accounts = _db.Accounts.ToList();

                return View(viewModel);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new BillViewModel());
            }
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BillViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid) return View(viewModel);


                var bill = new Bill();
                bill.Name = viewModel.Name;
                bill.Account = viewModel.Account;
                bill.AccountId = viewModel.AccountId;
                bill.AmountDue = viewModel.AmountDue;
                bill.DueDate = viewModel.DueDate;
                bill.Id = viewModel.Id;
                //bill.IsMandatory = viewModel.IsMandatory;
                bill.PaymentFrequency = viewModel.PaymentFrequency;
                
                _db.Entry(bill).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index");
            }
        }

        // GET: Bills/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Bill bill = _db.Bills.Find(id);
                if (bill == null)
                {
                    return HttpNotFound();
                }
                return View(bill);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new Bill());
            }
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Bill bill = _db.Bills.Find(id);
                _db.Bills.Remove(bill);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index");
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
