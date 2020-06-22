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

        //主頁顯示所需資料
        public ActionResult Index()
        {
            string ID = Session["account"].ToString();

            //篩選出自己的貼文，join需要內容的表，選取所需欄位
            var post1 = (from p in db.Post
                         where p.MemberID == ID
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName });

            //Relationship表利用大於1991判斷，把是自己好友的ID找出來
            string[] rid = db.Relationship.Where(m => m.MemberID == ID && m.FollowDate.Year > 1991).Select(m => m.RSMemberID).ToList().ToArray();

            //利用上面rid找出的好友，使用contains方法，篩選出是自己好友並且狀態不等於2(2表示不公開)的貼文
            var post2 = (from p in db.Post
                         where rid.Contains(p.MemberID) && p.StatusCategoryID != "2"
                         join m in db.Member on p.MemberID equals m.MemberID
                         join c in db.PostCategory on p.CategoryID equals c.CategoryID
                         join s in db.PostStatusCategory on p.StatusCategoryID equals s.StatusCategoryID
                         select new { p.PostID, p.PostContent, p.PostDate, p.EditDate, m.MemberName, c.CategoryName, s.StatusCategoryName });

            //將上面post1、post2、找出的發文合併和做發文時間的排序
            var post3 = post1.Union(post2).OrderByDescending(x => x.PostDate).ToList();

            //找出所有留言資料並join會員表(對貼文留言的會員資料)
            var comment = (from c in db.Comment
                           join m in db.Member on c.MemberID equals m.MemberID
                           select new { cmn = m.MemberName, c.PostID, c.Comment1 });

            //找出上面post3(自己與好友發過的文並且排序)每個貼文所對應發表的圖片放進去pps，留言也做相同事情(留言是透過上面commetn所抓取的)
            //join後面的into是使用Groupjoin的方法，是一種left join概念的作法
            var post = (from p in post3
                        join pp in db.PostPhoto on p.PostID equals pp.PostID into pps
                        join pc in comment on p.PostID equals pc.PostID into pcs
                        select new { p.PostID, p.PostContent, pps, p.PostDate, p.EditDate, p.MemberName, p.CategoryName, p.StatusCategoryName, pcs });

            //將post篩選過會員主頁應該顯示，所抓取資料丟進ViewNag給View做顯示頁面資料
            ViewBag.post = post.ToList();

            return View();
        }

        // 
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
