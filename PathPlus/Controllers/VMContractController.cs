using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PathPlus.Models;
using PathPlus.ViewModels;
namespace PathPlus.Controllers
{
    public class VMContractController : Controller
    {
        PathPlusEntities db = new PathPlusEntities();
        // GET: Contract
        public ActionResult Index()
        {
            VMAnnoTerm vm = new VMAnnoTerm()
            {
                Announcement = db.Announcement.ToList(),
                Term = db.Term.ToList()
            };
            return View(vm);
        }
    }
}