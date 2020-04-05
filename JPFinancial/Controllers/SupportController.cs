using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JPFinancial.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult GettingStarted()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}