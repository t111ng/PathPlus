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
    public class AnnouncementsController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();

        // GET: Announcements
        public ActionResult Index()
        {
            var announcement = db.Announcement.Include(a => a.AnnouncementStatusCategory);
            return View(announcement.ToList());
        }

        // GET: Announcements/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcement.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // GET: Announcements/Create
        public ActionResult Create()
        {
            ViewBag.StatusCategoryID = new SelectList(db.AnnouncementStatusCategory, "StatusCategoryID", "StatusCategoryName");
            return View();
        }

        // POST: Announcements/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnnouncementID,Content,PostDate,EditDate,RevokeDate,Editor,StatusCategoryID")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                db.Announcement.Add(announcement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusCategoryID = new SelectList(db.AnnouncementStatusCategory, "StatusCategoryID", "StatusCategoryName", announcement.StatusCategoryID);
            return View(announcement);
        }

        // GET: Announcements/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcement.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusCategoryID = new SelectList(db.AnnouncementStatusCategory, "StatusCategoryID", "StatusCategoryName", announcement.StatusCategoryID);
            return View(announcement);
        }

        // POST: Announcements/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AnnouncementID,Content,PostDate,EditDate,RevokeDate,Editor,StatusCategoryID")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(announcement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusCategoryID = new SelectList(db.AnnouncementStatusCategory, "StatusCategoryID", "StatusCategoryName", announcement.StatusCategoryID);
            return View(announcement);
        }

        // GET: Announcements/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcement.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Announcement announcement = db.Announcement.Find(id);
            db.Announcement.Remove(announcement);
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
    }
}
