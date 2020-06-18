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
    public class AdvertisersController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();

        // GET: Advertisers
        public ActionResult Index()
        {
            return View(db.Advertisers.ToList());
        }

        // GET: Advertisers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisers advertisers = db.Advertisers.Find(id);
            if (advertisers == null)
            {
                return HttpNotFound();
            }
            return View(advertisers);
        }

        // GET: Advertisers/Create
        public ActionResult Create()
        {
            Advertisers advertisers = new Advertisers();
            SelfFeature sfe = new SelfFeature();
            advertisers.CompanyID = sfe.GetID("Advertisers");
            return View(advertisers);
        }

        // POST: Advertisers/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需b
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyID,CompanyName,ContactName,Phone,Address,Mail")] Advertisers advertisers)
        {
            SelfFeature sfe = new SelfFeature();
            advertisers.CompanyID = sfe.GetID("Advertisers");
            if (ModelState.IsValid)
            {
                db.Advertisers.Add(advertisers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(advertisers);
        }

        // GET: Advertisers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisers advertisers = db.Advertisers.Find(id);
            if (advertisers == null)
            {
                return HttpNotFound();
            }
            return View(advertisers);
        }

        // POST: Advertisers/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyID,CompanyName,ContactName,Phone,Address,Mail")] Advertisers advertisers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(advertisers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(advertisers);
        }

        // GET: Advertisers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisers advertisers = db.Advertisers.Find(id);
            if (advertisers == null)
            {
                return HttpNotFound();
            }
            return View(advertisers);
        }

        // POST: Advertisers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Advertisers advertisers = db.Advertisers.Find(id);
            db.Advertisers.Remove(advertisers);
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
