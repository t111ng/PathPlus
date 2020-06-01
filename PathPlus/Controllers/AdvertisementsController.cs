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
    public class AdvertisementsController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();

        // GET: Advertisements
        public ActionResult Index()
        {
            var advertisement = db.Advertisement.Include(a => a.AdvertisemenStatusCategory).Include(a => a.Advertisers);
            return View(advertisement.ToList());
        }

        // GET: Advertisements/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisement.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            return View(advertisement);
        }

        // GET: Advertisements/Create
        public ActionResult Create()
        {
            ViewBag.AdStatusCategoryID = new SelectList(db.AdvertisemenStatusCategory, "AdStatusCategoryID", "AdStatusCategoryName");
            ViewBag.CompanyID = new SelectList(db.Advertisers, "CompanyID", "CompanyName");
            return View();
        }

        // POST: Advertisements/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdvertisementID,AdText,City,AgeRange,Gender,Money,StartDate,Limitation,ExpireDate,CompanyID,AdStatusCategoryID")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                db.Advertisement.Add(advertisement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdStatusCategoryID = new SelectList(db.AdvertisemenStatusCategory, "AdStatusCategoryID", "AdStatusCategoryName", advertisement.AdStatusCategoryID);
            ViewBag.CompanyID = new SelectList(db.Advertisers, "CompanyID", "CompanyName", advertisement.CompanyID);
            return View(advertisement);
        }

        // GET: Advertisements/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisement.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdStatusCategoryID = new SelectList(db.AdvertisemenStatusCategory, "AdStatusCategoryID", "AdStatusCategoryName", advertisement.AdStatusCategoryID);
            ViewBag.CompanyID = new SelectList(db.Advertisers, "CompanyID", "CompanyName", advertisement.CompanyID);
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdvertisementID,AdText,City,AgeRange,Gender,Money,StartDate,Limitation,ExpireDate,CompanyID,AdStatusCategoryID")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(advertisement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdStatusCategoryID = new SelectList(db.AdvertisemenStatusCategory, "AdStatusCategoryID", "AdStatusCategoryName", advertisement.AdStatusCategoryID);
            ViewBag.CompanyID = new SelectList(db.Advertisers, "CompanyID", "CompanyName", advertisement.CompanyID);
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisement.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Advertisement advertisement = db.Advertisement.Find(id);
            db.Advertisement.Remove(advertisement);
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
