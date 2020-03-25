using System.Web.Mvc;
using JPFData;
using JPFData.Managers;
using Microsoft.AspNet.Identity;

namespace JPFinancial.Controllers
{
    public class HomeController : Controller
    {
        private readonly BillManager _billManager;

        public HomeController()
        {
            _billManager = new BillManager();
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return View();

            Global.Instance.User.Id = User.Identity.GetUserId();

            var accountManager = new AccountManager();
            accountManager.CheckAndCreatePoolAccount();

            _billManager.UpdateBillDueDates();


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
    }
}