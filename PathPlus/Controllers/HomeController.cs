using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PathPlus.Models;
namespace PathPlus.Controllers
{
    public class HomeController : Controller
    {
        PathPlusEntities db = new PathPlusEntities();
        // GET: Home
        public ActionResult Index()
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Private()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult NewPost()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost(string Pcontent, string CID, string SCID, HttpPostedFileBase[] photo)
        //
        {



            Post newpost = new Post();
            PostPhoto newphoto = new PostPhoto();

            SelfFeature sf = new SelfFeature();
            string PID = sf.GetID("Post");

            string fileName = "";
            bool b = true;

            if (b == true)
            {
                newpost.PostID = PID;
                newpost.PostContent = Pcontent;
                newpost.PostDate = DateTime.Now;
                newpost.MemberID = Session["account"].ToString();
                newpost.CategoryID = CID;
                newpost.StatusCategoryID = SCID;

                try
                {
                    db.Post.Add(newpost);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                b = false;
            }

            newphoto.PostID = PID;

            for (int i = 0; i < photo.Length; i++)
            {
                if (photo[i] != null)
                {
                    if (photo[i].ContentLength > 0)
                    {

                        fileName = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "").Replace("上午", "").Replace("下午", "") + (i + 1).ToString() + ".jpg";
                        photo[i].SaveAs(Server.MapPath("~/Photo/" + fileName));                      
                        newphoto.Photo =fileName;

                        db.PostPhoto.Add(newphoto);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

       
    }
}