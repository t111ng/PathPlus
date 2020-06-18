using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PathPlus.Controllers
{
    public class AdministratorHomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Autologout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "AdministratorLogin");
        }
    }
}