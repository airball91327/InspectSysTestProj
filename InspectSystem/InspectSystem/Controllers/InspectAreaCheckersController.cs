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
    //[Authorize(Roles = "Admin")]
    public class InspectAreaCheckersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectAreaCheckers
        public ActionResult Index()
        {
            var inspectAreaCheckers = db.inspectAreaCheckers.Include(i => i.InspectAreas);
            return View(inspectAreaCheckers.ToList());
        }

        // GET: InspectAreaCheckers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectAreaChecker inspectAreaChecker = db.inspectAreaCheckers.Find(id);
            if (inspectAreaChecker == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
            return View(inspectAreaChecker);
        }

        // POST: InspectAreaCheckers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AreaID,CheckerID,CheckerName")] InspectAreaChecker inspectAreaChecker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectAreaChecker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
            return View(inspectAreaChecker);
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
