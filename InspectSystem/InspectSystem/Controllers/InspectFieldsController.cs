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
    public class InspectFieldsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectFields
        public ActionResult Index()
        {
            return PartialView(db.InspectFields.ToList());
        }

        public ActionResult Search(int acid, int itemid)
        {
            var SearchResult = db.InspectFields
                                 .Where(i => i.ACID == acid &&
                                             i.ItemID == itemid);
            ViewBag.ACID = acid;
            ViewBag.ItemID = itemid;
            return PartialView(SearchResult.ToList());
        }

        // GET: InspectFields/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(id);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }

        // GET: InspectFields/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: InspectFields/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ItemID,FieldID,FieldName,DataType,UnitOfData,MinValue,MaxValue")] InspectFields inspectFields)
        {
            if (ModelState.IsValid)
            {
                db.InspectFields.Add(inspectFields);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(inspectFields);
        }

        // GET: InspectFields/Edit/5
        public ActionResult Edit(int? ACID, int? itemID, int? fieldID)
        {
            if (ACID == null || itemID == null || fieldID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(ACID, itemID, fieldID);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }

        // POST: InspectFields/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InspectFields inspectFields)
        {
            var ACID = inspectFields.ACID;
            var itemID = inspectFields.ItemID;

            if (ModelState.IsValid)
            {
                db.Entry(inspectFields).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Search",new { acid = ACID, itemid = itemID });
            }
            return RedirectToAction("Search");
        }

        // GET: InspectFields/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(id);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }

        // POST: InspectFields/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectFields inspectFields = db.InspectFields.Find(id);
            db.InspectFields.Remove(inspectFields);
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
