using JPFinancial.Models;
using JPFinancial.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace JPFinancial.Controllers
{
    public class BillsController : Controller
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

        // GET: Bills/Create
        public ActionResult Create()
        {

            var viewModel = new CreateBillViewModel
            {
                Accounts = _db.Accounts.ToList()
            };

            return View(viewModel);
        }

        // POST: Bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBillViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var account = _db.Accounts.Single(a => a.Id == viewModel.AccountId);
                var accountId = account.Id;

                var bill = new Bill
                {
                    Name = viewModel.Name,
                    IsMandatory = viewModel.IsMandatory,
                    AmountDue = viewModel.AmountDue,
                    DueDate = Convert.ToDateTime(viewModel.DueDate),
                    PaymentFrequency = viewModel.PaymentFrequency,
                    AccountId = accountId,
                    Account = account
                };

                _db.Bills.Add(bill);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        public static IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Account> accounts, int selectedId)
        {
            return accounts.OrderBy(account => account.Name).Select(account => new SelectListItem
            {
                Selected = (account.Id == selectedId),
                Text = account.Name,
                Value = account.Id.ToString()
            });
        }

        // GET: Bills/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillId,Name,IsMandatory,AccountNumber,Balance,DueDate,PaymentFrequency,IsLate")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(bill).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bill);
        }

        // GET: Bills/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bill bill = _db.Bills.Find(id);
            _db.Bills.Remove(bill);
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
