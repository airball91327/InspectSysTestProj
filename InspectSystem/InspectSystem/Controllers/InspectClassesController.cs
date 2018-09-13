using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectClassesController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectClasses
        public ActionResult Index()
        {
            ViewBag.Message = "Your InspectClasses Index page.";
            return View(db.InspectClasses.ToList());
        }

        // GET: InspectClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectClasses inspectClasses = db.InspectClasses.Find(id);
            if (inspectClasses == null)
            {
                return HttpNotFound();
            }
            return View(inspectClasses);
        }

        // GET: InspectClasses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InspectClasses/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassID,ClassName")] InspectClasses inspectClasses)
        {
            if (ModelState.IsValid)
            {
                db.InspectClasses.Add(inspectClasses);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inspectClasses);
        }

        // GET: InspectClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectClasses inspectClasses = db.InspectClasses.Find(id);
            if (inspectClasses == null)
            {
                return HttpNotFound();
            }
            return View(inspectClasses);
        }

        // POST: InspectClasses/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassID,ClassName")] InspectClasses inspectClasses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectClasses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inspectClasses);
        }

        // GET: InspectClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectClasses inspectClasses = db.InspectClasses.Find(id);
            if (inspectClasses == null)
            {
                return HttpNotFound();
            }
            return View(inspectClasses);
        }

        // POST: InspectClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectClasses inspectClasses = db.InspectClasses.Find(id);
            db.InspectClasses.Remove(inspectClasses);
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
