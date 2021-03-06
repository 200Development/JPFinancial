﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Models.JPFinancial;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class CreditCardsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CreditCards
        public ActionResult Index()
        {
            return View(db.CreditCards.ToList());
        }

        // GET: CreditCards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            return View(creditCard);
        }

        // GET: CreditCards/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreditCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CurrentBalance,RemainingStatementBalance,CreditLimit,PurchaseApr,CashAdvanceApr,GracePeriodDays,EndOfCycleDay,NextPaymentDue,CashAdvanceBalance,CashAdvanceLimit")] CreditCard creditCard)
        {
            if (!ModelState.IsValid) return View(creditCard);


            db.CreditCards.Add(creditCard);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: CreditCards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            return View(creditCard);
        }

        // POST: CreditCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CurrentBalance,RemainingStatementBalance,CreditLimit,PurchaseApr,CashAdvanceApr,GracePeriodDays,EndOfCycleDay,NextPaymentDue,CashAdvanceBalance,CashAdvanceLimit")] CreditCard creditCard)
        {
            if (!ModelState.IsValid) return View(creditCard);


            db.Entry(creditCard).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CreditCards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            return View(creditCard);
        }

        // POST: CreditCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CreditCard creditCard = db.CreditCards.Find(id);
            db.CreditCards.Remove(creditCard);
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
