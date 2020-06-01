using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PathPlus.Models;
namespace PathPlus.Controllers
{
    public class ManagerLoginController : Controller
    {
        PathPlusEntities db = new PathPlusEntities();
        // GET: ManagerLogin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string account, string pwd)
        {
            var result = db.Administrator.Where(m => m.Account == account)
                        .Where(m=>m.Password == pwd).FirstOrDefault();

            if (result != null)
            {
                
                Session["account"] = result.AdministratorID;
                ViewBag.account= result.AdministratorID; ;

                return View();
            }

            ViewBag.Msg = "帳號或密碼錯誤";

            return View();
        }
    }
}