using System.Web.Mvc;
using System.Web.Security;
using JPFData;
using Microsoft.AspNet.Identity;

namespace JPFinancial.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Global.Instance.User.Id = User.Identity.GetUserId();
            }
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