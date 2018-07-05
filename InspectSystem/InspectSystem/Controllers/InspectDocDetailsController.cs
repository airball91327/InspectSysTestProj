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
    public class InspectDocDetailsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocDetails
        public ActionResult Index(int? areaID)
        {
            int classID = 1; //Default value
            var findACID = db.ClassesOfAreas.Where(c => c.AreaID == areaID &&
                                                    c.ClassID == classID);
            int ACID = 0;
            foreach (var item in findACID) {
                ACID = item.ACID;
            }
            var inspectFields = db.InspectFields.Include(i => i.ClassesOfAreas)
                                                .Include(i => i.ClassesOfAreas.InspectAreas)
                                                .Include(i => i.ClassesOfAreas.InspectClasses);
            var fieldsForACID = inspectFields.Where(i => i.ACID == ACID).ToList();
            var itemsForACID = db.InspectItems.Where(i => i.ACID == ACID).ToList();

            return View(new InspectDocDetailsViewModels() {
                InspectFields = fieldsForACID,
                InspectItems = itemsForACID
            });
        }

        // GET: InspectDocDetails/SelectAreas
        public ActionResult SelectAreas()
        {
            return View(db.InspectAreas.ToList());
        }

        // GET: InspectDocDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetails inspectDocDetails = db.InspectDocDetails.Find(id);
            if (inspectDocDetails == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetails);
        }

        // GET: InspectDocDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InspectDocDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocID,ClassID,ItemID,FieldID,AreaID,AreaName,ClassName,ItemName,FieldName,UnitOfData,Value,IsFunctional,ErrorDescription,RepairDocID")] InspectDocDetails inspectDocDetails)
        {
            if (ModelState.IsValid)
            {
                db.InspectDocDetails.Add(inspectDocDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inspectDocDetails);
        }

        // GET: InspectDocDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetails inspectDocDetails = db.InspectDocDetails.Find(id);
            if (inspectDocDetails == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetails);
        }

        // POST: InspectDocDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocID,ClassID,ItemID,FieldID,AreaID,AreaName,ClassName,ItemName,FieldName,UnitOfData,Value,IsFunctional,ErrorDescription,RepairDocID")] InspectDocDetails inspectDocDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inspectDocDetails);
        }

        // GET: InspectDocDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetails inspectDocDetails = db.InspectDocDetails.Find(id);
            if (inspectDocDetails == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetails);
        }

        // POST: InspectDocDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectDocDetails inspectDocDetails = db.InspectDocDetails.Find(id);
            db.InspectDocDetails.Remove(inspectDocDetails);
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
