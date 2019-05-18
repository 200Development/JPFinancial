using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly BillManager _billManager = new BillManager();

        // GET: Bills
        public ActionResult Index()
        {
            try
            {
                Logger.Instance.DataFlow("Index");
                BillViewModel billVM = new BillViewModel();
                billVM = _billManager.GetAllBills(billVM);


                return View(billVM);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // GET: Bills/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                Logger.Instance.DataFlow($"Details");
                if (id == null)
                {
                    Logger.Instance.Debug("Bill ID is null");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                BillViewModel billVM = new BillViewModel();
                billVM.Bill = _billManager.GetBill(id);

                if (billVM.Bill != null)
                {
                    return View(billVM);
                }


                Logger.Instance.Debug("Returned Bill is null - (error)");
                return HttpNotFound();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new BillViewModel());
            }
        }

        // GET: Bills/Create
        public ActionResult Create()
        {
            try
            {
                BillViewModel billVM = new BillViewModel();
                billVM.Accounts = _billManager.GetAllAccounts();


                return View(billVM);
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
                if (!_billManager.Create(billVM)) return View(billVM);


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new BillViewModel());
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
                BillViewModel billVM = new BillViewModel();
                billVM.Bill = _billManager.GetBill(id);
                Logger.Instance.DataFlow($"Pull Bill with ID {id} from DB and set to BillViewModel.Bill");


                if (billVM.Bill == null)
                {
                    return HttpNotFound();
                }
                billVM.Bill.Account = _billManager.GetAllAccounts().Single(a => a.Id == billVM.Bill.AccountId);
                billVM.Accounts = _billManager.GetAllAccounts();
                Logger.Instance.DataFlow($"Pull Bill.Account with ID {billVM.Bill.AccountId} from DB and set to BillViewModel.Bill.Account");


                return View(billVM);
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
        public ActionResult Edit(BillViewModel billVM)
        {
            try
            {
                if (!ModelState.IsValid) return View(billVM);
                if (_billManager.Edit(billVM))
                    return RedirectToAction("Index");


                return View(billVM);
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
                Bill bill = _billManager.GetBill(id);
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
                if (_billManager.Delete(id))
                    return RedirectToAction("Index");

                // Send Bill back to Delete View if delete failed
                Bill bill = _billManager.GetBill(id);
                if (bill == null)
                {
                    return HttpNotFound();
                }


                return View(bill);
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
