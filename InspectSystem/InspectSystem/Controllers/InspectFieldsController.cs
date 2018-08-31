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
    public class InspectFieldsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectFields
        public ActionResult Index()
        {
            return PartialView(db.InspectFields.ToList());
        }

        // GET: InspectFields/Search
        /* Use ACID and ItemID to search the fields. */
        public ActionResult Search(int acid, int itemid)
        {
            var SearchResult = db.InspectFields
                                 .Where(i => i.ACID == acid &&
                                             i.ItemID == itemid);
            ViewBag.ACID = acid;
            ViewBag.ItemID = itemid;
            return PartialView(SearchResult.ToList());
        }

        /* Unused code
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
        */

        // GET: InspectFields/Create
        public ActionResult Create(int acid, int itemid)
        {
            /* Pass the ACID, ItemID, ItemName for View to print. */
            ViewBag.CreateACID = acid;
            ViewBag.CreateItemID = itemid;
            ViewBag.ItemNameForCreate = db.InspectItems.Find(acid, itemid).ItemName;

            /* Set the default values for create field. */
            InspectFields inspectFields = new InspectFields
            {
                ACID = acid,
                ItemID = itemid,
                MaxValue = 0,
                MinValue = 0,
                FieldStatus = true,
                IsRequired = true
            };

            return PartialView(inspectFields);
        }

        // POST: InspectFields/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InspectFields inspectFields)
        {
            int ACID = inspectFields.ACID;
            int itemID = inspectFields.ItemID;

            int fieldCount = db.InspectFields.Count(fc => fc.ACID == ACID && fc.ItemID == itemID);
            int fieldID = fieldCount + 1;

            inspectFields.FieldID = fieldID;

            if (ModelState.IsValid)
            {
                db.InspectFields.Add(inspectFields);
                db.SaveChanges();
                return RedirectToAction("Search", new { acid = ACID, itemid = itemID });
            }
            return RedirectToAction("Search", new { acid = ACID, itemid = itemID });
        }

        // GET: InspectFields/Edit/5
        public ActionResult Edit(int? ACID, int? itemID, int? fieldID)
        {

            ViewBag.ItemNameForEdit = db.InspectItems.Find(ACID, itemID).ItemName;

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
            return RedirectToAction("Search", new { acid = ACID, itemid = itemID });
        }

        /* Unused code
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
        */

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
