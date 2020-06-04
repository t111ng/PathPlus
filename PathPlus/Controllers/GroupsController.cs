using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PathPlus.Models;

namespace PathPlus.Controllers
{
    public class GroupsController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();

        // GET: Groups
        public ActionResult Index()
        {
            var group = db.Group.Include(g => g.GroupPrivateCategory).Include(g => g.Member);
            return View(group.ToList());
        }

        // GET: Groups/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName");
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName");
            return View();
        }

        // POST: Groups/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,GroupName,Photo,GroupIntroduction,GroupInformation,CreateDate,MemberID,PrivateCategoryID")] Group group)
        {


            group.CreateDate = DateTime.Now;

            var id = db.Group.OrderByDescending(p => p.GroupID).FirstOrDefault().GroupID;
            int nid = int.Parse(id.Substring(4)) + 1;
            id = "G" + DateTime.Now.Year.ToString().Substring(1, 3);
            string sid = nid.ToString();
            for (int i = 0; i < (11 - sid.Length); i++)
            {
                id += 0;
            }
            id += sid;
            group.GroupID = id;


            group.MemberID = Session["account"].ToString();

            Group grp = db.Group.Where(m => m.GroupName == group.GroupName).FirstOrDefault();
            //var grp = from a in db.Group where a.GroupName == group.GroupName select a;

            if (grp != null) return RedirectToAction("DupGrp");

            GroupManagement groupManagement = new GroupManagement();
            groupManagement.MemberID = Session["accuont"].ToString();
            groupManagement.GroupID = id;
            groupManagement.ManageDate = DateTime.Now;
            groupManagement.AuthorityCategoryID = "0";



            Post post = new Post();
            var pid = db.Group.OrderByDescending(p => p.GroupID).FirstOrDefault().GroupID;
            int npid = int.Parse(id.Substring(4)) + 1;
            pid = "P" + DateTime.Now.Year.ToString().Substring(1, 3);
            string spid = npid.ToString();
            for (int i = 0; i < (11 - spid.Length); i++)
            {
                pid += 0;
            }
            pid += spid;
            post.PostID = pid;
            post.PostContent = "我新增了一個群組：" + group.GroupName + "。歡迎大家加入～";
            post.PostDate = DateTime.Now;
            post.EditDate = DateTime.Now;
            post.MemberID = Session["account"].ToString();
            //post.CategoryID = "0";
            post.StatusCategoryID = group.PrivateCategoryID;



            if (ModelState.IsValid)
            {
                db.Group.Add(group);
                db.SaveChanges();
                db.GroupManagement.Add(groupManagement);
                db.SaveChanges();
                db.Post.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName", group.PrivateCategoryID);
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", group.MemberID);
            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName", group.PrivateCategoryID);
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", group.MemberID);
            return View(group);
        }

        // POST: Groups/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,GroupName,Photo,GroupIntroduction,GroupInformation,CreateDate,MemberID,PrivateCategoryID")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PrivateCategoryID = new SelectList(db.GroupPrivateCategory, "PrivateCategoryID", "PrivateCategoryName", group.PrivateCategoryID);
            ViewBag.MemberID = new SelectList(db.Member, "MemberID", "MemberName", group.MemberID);
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Group group = db.Group.Find(id);
            db.Group.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DupGrp()
        {
            //Response.Write("<h2>帳號重複</h2><a href='Index'>Back to List</a>");

            return View();
        }
    }
}
