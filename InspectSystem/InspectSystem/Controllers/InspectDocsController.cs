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
    public class InspectDocsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocs
        public ActionResult Index()
        {
            var inspectDocs = db.InspectDocs.Include(i => i.InspectAreas).Include(i => i.InspectFlowStatusTable);
            var editingDocs = inspectDocs.Where(i => i.FlowStatusID == 3); // 編輯中文件
            return View(editingDocs.ToList());
        }

        // GET: InspectDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocs inspectDocs = db.InspectDocs.Find(id);
            if (inspectDocs == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectDocs.AreaID);
            ViewBag.FlowStatusID = new SelectList(db.InspectFlowStatusTable, "FlowStatusID", "FlowStatusName", inspectDocs.FlowStatusID);
            return View(inspectDocs);
        }

        // POST: InspectDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocID,Date,EndTime,AreaID,WorkerID,WorkerName,CheckerID,CheckerName,FlowStatusID")] InspectDocs inspectDocs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectDocs.AreaID);
            ViewBag.FlowStatusID = new SelectList(db.InspectFlowStatusTable, "FlowStatusID", "FlowStatusName", inspectDocs.FlowStatusID);
            return View(inspectDocs);
        }

        // GET: InspectDocs/Create
        public ActionResult Create()
        {
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            return View();
        }

        // POST: InspectDocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Date,AreaID,WorkerID,WorkerName")] InspectDocs inspectDocs)
        {
            if (ModelState.IsValid)
            {
                var findAreaChecker = db.InspectAreaCheckers.Where(i => i.AreaID == inspectDocs.AreaID).First();
                /* Set doc details.*/
                inspectDocs.DocID = System.Convert.ToInt32(inspectDocs.Date) * 100 + inspectDocs.AreaID;
                inspectDocs.AreaName = inspectDocs.InspectAreas.AreaName;
                inspectDocs.CheckerID = findAreaChecker.CheckerID;
                inspectDocs.CheckerName = findAreaChecker.CheckerName;
                inspectDocs.FlowStatusID = 3;        // Default flow status:"編輯中"

                db.InspectDocs.Add(inspectDocs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectDocs.AreaID);
            return View(inspectDocs);
        }

        //// GET: InspectDocs1/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    InspectDocs inspectDocs = db.InspectDocs.Find(id);
        //    if (inspectDocs == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(inspectDocs);
        //}

        //// POST: InspectDocs1/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    InspectDocs inspectDocs = db.InspectDocs.Find(id);
        //    db.InspectDocs.Remove(inspectDocs);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
