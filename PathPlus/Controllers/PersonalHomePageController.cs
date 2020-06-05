using PathPlus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PathPlus.Controllers
{
    public class PersonalHomePageController : Controller
    {
        // GET: PersonalHomePage
        PathPlusEntities db = new PathPlusEntities();

        // GET: Post
        public ActionResult Index()
        {

          
            string MID = Session["account"].ToString();
            string[] selfpost = db.Post.Where(p => p.MemberID == MID).Select(p => p.PostID).ToList().ToArray();
            PersonalViewModel vm = new PersonalViewModel()
            {
                //select* from member where MemberID = "M02000000000001"
                member = db.Member.Where(m => m.MemberID == MID).ToList(),
                post = db.Post.Where(p => p.MemberID == MID).OrderByDescending(m => m.PostDate).ToList(),

                postPhoto = db.PostPhoto.Where(p => selfpost.Contains(p.PostID)).OrderByDescending(p=>p.PostID).ToList()

            };
            ViewBag.pp = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().PersonalProfile;
            ViewBag.mn = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().MemberName;
            ViewBag.em = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().Mail;
            ViewBag.ph = db.Member.Where(m => m.MemberID == MID).FirstOrDefault().Photo;

          
            return View(vm);
        }

        //Member member = new Member();
        public ActionResult Editpersonal(string id)
        {
            db.Member.Where(m => m.MemberID == id).ToList();

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editpersonal(Member member)
        {
            string a = Session["account"].ToString();
            var m = db.Member.Where(o => o.MemberID == a).FirstOrDefault();
            if(member.MemberName!=null)
                m.MemberName = member.MemberName;
            if (member.PersonalProfile != null)
                m.PersonalProfile = member.PersonalProfile;
            if (member.Mail != null)
                m.Mail = member.Mail;
            if (member.Address != null)
                m.Address = member.Address;
            if (member.Gender != null)
                m.Gender = member.Gender;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult selfphoto(HttpPostedFileBase photo)
        {
            string a = Session["account"].ToString();
            var m = db.Member.Where(o => o.MemberID == a).FirstOrDefault();

            if (photo.ContentLength > 0)
            {
                string fileName = "";
                fileName = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") .ToString() + "p.jpg";
                photo.SaveAs(Server.MapPath("~/Selfphoto/" + fileName));
                m.Photo = fileName;              
                db.SaveChanges();
            }

            return RedirectToAction("Index");


        }
    
      public ActionResult viewpost()
        {


            return View();
        }
    
    }

    
}