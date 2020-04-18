using System.Web.Mvc;
using JPFData;
using JPFData.Managers;
using Microsoft.AspNet.Identity;

namespace JPFinancial.Controllers
{
    public class HomeController : Controller
    {
        private bool _isUserIdSet;

        public HomeController()
        {
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Identity");
            if(!_isUserIdSet)
                SetGlobalUser();


            var accountManager = new AccountManager();
            accountManager.CheckAndCreatePoolAccount();
            accountManager.CheckAndCreateEmergencyFund();
            accountManager.CheckAndCreateAddNewAccount();

            var billManager = new BillManager();
            billManager.UpdateBillDueDates();


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void SetGlobalUser()
        {
            if (!User.Identity.IsAuthenticated) return;

            Global.Instance.User.Id = User.Identity.GetUserId();
            _isUserIdSet = true;
        }
    }
}