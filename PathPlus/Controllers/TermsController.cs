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
    public class TermsController : Controller
    {
        private PathPlusEntities db = new PathPlusEntities();

        // GET: Terms
        public ActionResult Index()
        {
            var term = db.Term.Include(t => t.TermStatusCategory);
            return View(term.ToList());
        }

        // GET: Terms/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term term = db.Term.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            return View(term);
        }

        // GET: Terms/Create
        public ActionResult Create()
        {
            ViewBag.StatusCategoryID = new SelectList(db.TermStatusCategory, "StatusCategoryID", "StatusCategoryName");
            return View();
        }

        // POST: Terms/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TermID,Content,PostDate,EditDate,RevokeDate,Editor,StatusCategoryID")] Term term)
        {
            if (ModelState.IsValid)
            {
                db.Term.Add(term);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusCategoryID = new SelectList(db.TermStatusCategory, "StatusCategoryID", "StatusCategoryName", term.StatusCategoryID);
            return View(term);
        }

        // GET: Terms/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term term = db.Term.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusCategoryID = new SelectList(db.TermStatusCategory, "StatusCategoryID", "StatusCategoryName", term.StatusCategoryID);
            return View(term);
        }

        // POST: Terms/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TermID,Content,PostDate,EditDate,RevokeDate,Editor,StatusCategoryID")] Term term)
        {
            if (ModelState.IsValid)
            {
                db.Entry(term).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusCategoryID = new SelectList(db.TermStatusCategory, "StatusCategoryID", "StatusCategoryName", term.StatusCategoryID);
            return View(term);
        }

        // GET: Terms/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term term = db.Term.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            return View(term);
        }

        // POST: Terms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Term term = db.Term.Find(id);
            db.Term.Remove(term);
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
