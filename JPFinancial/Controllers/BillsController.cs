using System;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly BillManager _billManager = new BillManager();
        private readonly AccountManager _accountManager = new AccountManager();

        // GET: Bills
        public ActionResult Index()
        {
            try
            {
                Logger.Instance.DataFlow("Index");
                BillViewModel billVM = new BillViewModel();
                billVM.Accounts = _accountManager.GetAllAccounts();
                billVM.Bills = _billManager.GetAllBills();
                billVM.Metrics = _billManager.GetBillMetrics();

                return View(billVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new BillViewModel());
            }
        }

        public ActionResult Create(BillViewModel billVM)
        {
            try
            {
                // Get hidden values that are set for dropdowns on change event
                var selectedAccountId = Request.Form["selectedAccountId"];
                billVM.Bill.AccountId = Convert.ToInt32(selectedAccountId);

                var selectedFrequencyId = Convert.ToInt32(Request.Form["selectedFrequencyId"]);
                if (selectedFrequencyId > 0) billVM.Bill.PaymentFrequency = (FrequencyEnum) selectedFrequencyId;
                

                if (!_billManager.Create(billVM.Bill)) return View("Error");


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        public JsonResult EditBill(Bill bill)
        {
            try
            {
                return Json(_billManager.Edit(bill) ? "Success" : "Failed");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        public JsonResult DeleteBill(Bill bill)
        {
            try
            {
                return bill.Id > 0 ? Json(_billManager.Delete(bill.Id) ? "Success" : "Failed") : Json("ID is invalid");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
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
