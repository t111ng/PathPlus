using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PathPlus.Models;

using System.Configuration;
using System.Data.SqlClient;

namespace PathPlus.Controllers
{
    public class PostsController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();




        //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
        //SqlCommand Cmd = new SqlCommand();
        //SqlDataAdapter adp = new SqlDataAdapter();

        //private DataTable querySql(string sql)
        //{
        //    Cmd.CommandText = sql;
        //    Cmd.Connection = Conn;
        //    adp.SelectCommand = Cmd;
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    return ds.Tables[0];
        //}




        // GET: Posts
        public ActionResult Index()
        {
            //string ID = Session["id"].ToString();
            //var post = db.Post.Where(p => p.MemberID == @ID).Include(p => p.Member).Include(p => p.PostStatusCategory).Include(p => p.PostCategory);



            //var post = (from p in db.Post select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, p.MemberID, p.CategoryID, p.StatusCategoryID });

            string ID = Session["account"].ToString();

            var post1 = (from p in db.Post
                         where p.MemberID == ID
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName });



            string[] rid = db.Relationship.Where(m => m.MemberID == ID && m.FollowDate.Year > 1773).Select(m => m.RSMemberID).ToList().ToArray();
            var post2 = (from p in db.Post
                         where rid.Contains(p.MemberID) && p.StatusCategoryID != "2"
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName });

            var post3 = post1.Union(post2).OrderByDescending(x => x.PostDate).ToList();

            var comment = (from c in db.Comment
                           join m in db.Member on c.MemberID equals m.MemberID
                           select new { cmn = m.MemberName, c.PostID, c.Comment1 });
            var post = (from p in post3
                        join pp in db.PostPhoto on p.PostID equals pp.PostID into pps
                        join pc in comment on p.PostID equals pc.PostID into pcs
                        select new { p.PostID, p.PostContent, pps, p.PostDate, p.EditDate, p.MemberName, p.CategoryName, p.StatusCategoryName, pcs });

            ViewBag.post = post.ToList();

            //sql
            //string sql = "select * from Post where MemberID=@ID";
            //Cmd.Parameters.AddWithValue("@ID", Session["id"].ToString());
            //string sql = "select p.PostContent, p.PostDate, p.EditDate, m.MemberName, s.StatusCategoryName, c.CategoryName from Post as p inner join Member as m inner join StatusCategory as s inner join CategoryID as c";
            //DataTable dt = querySql(sql);
            //return View(dt);


            //linq
            return View();

            //return View(post.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName");
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName");
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostID,PostContent,PostDate,EditDate,MemberID,CategoryID,StatusCategoryID")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Post.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", post.MemberID);
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName", post.StatusCategoryID);
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", post.MemberID);
            ViewBag.StatusCategoryID = new SelectList(db.PostStatusCategory, "StatusCategoryID", "StatusCategoryName", post.StatusCategoryID);
            ViewBag.CategoryID = new SelectList(db.PostCategory, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // POST: Posts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post)
        {
            string ID = Session["account"].ToString();
            var m = db.Post.Where(p => p.MemberID == ID && p.PostID == post.PostID).FirstOrDefault();

            m.PostContent = post.PostContent == null ? m.PostContent : post.PostContent;
            m.CategoryID = post.CategoryID == null ? m.CategoryID : post.CategoryID;
            m.StatusCategoryID = post.StatusCategoryID == null ? m.StatusCategoryID : post.StatusCategoryID;
            m.EditDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

        // GET: Posts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var member = (from p in db.Post
                          where p.PostID == id
                          select new { p.MemberID });

            var comment = db.Comment.Where(c => c.PostID == id);
            
            db.Comment.RemoveRange(comment);
            db.SaveChanges();


            var postphoto = db.PostPhoto.Where(p => p.PostID == id);

            db.PostPhoto.RemoveRange(postphoto);
            db.SaveChanges();

            Post post = db.Post.Find(id);
            db.Post.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
