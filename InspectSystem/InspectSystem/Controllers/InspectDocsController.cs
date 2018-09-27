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
