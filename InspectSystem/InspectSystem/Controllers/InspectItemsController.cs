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
    public class InspectItemsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectItems
        public ActionResult Index()
        {
            //Get the data from table "InspectItems", "InspectAreas", and "InspectClasses"
            var InspectItems = db.InspectItems
                                 .Include(i => i.InspectAreas)
                                 .Include(i => i.InspectClasses);

            //DropDownList for user to select area and class
            ViewBag.AreaList = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            ViewBag.ClassList = new SelectList(db.InspectClasses, "ClassID", "ClassName");

            return View(InspectItems.ToList());
        }

        public ActionResult SearchItems()
        {
            //尚未處理的例外:SearchResult回傳null => 查無資料

            int AreaListValue = System.Convert.ToInt32(Request.Form["AreaList"]);
            int ClassListValue = System.Convert.ToInt32(Request.Form["ClassList"]);
            TempData["TestValue"] = AreaListValue + "/" + ClassListValue;

            var InspectItems = db.InspectItems
                                 .Include(i => i.InspectAreas)
                                 .Include(i => i.InspectClasses);
            var SearchResult = InspectItems
                               .Where(s => s.AreaID == AreaListValue &&
                                           s.ClassID == ClassListValue);
            TempData["SearchResult"]  = SearchResult.ToList();

            return RedirectToAction("Index");
        }

        // GET: InspectItems/Details/5
        public ActionResult Details(int? id, int? Itemid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectItems inspectItems = db.InspectItems.Find(id, Itemid);
            if (inspectItems == null)
            {
                return HttpNotFound();
            }
            return View(inspectItems);
        }

        // GET: InspectItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InspectItems/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ItemID,AreaID,ClassID,ItemName,ItemStatus")] InspectItems inspectItems)
        {
            if (ModelState.IsValid)
            {
                db.InspectItems.Add(inspectItems);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inspectItems);
        }

        // GET: InspectItems/Edit/5
        public ActionResult Edit(int? id, int? Itemid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectItems inspectItems = db.InspectItems.Find(id, Itemid);
            if (inspectItems == null)
            {
                return HttpNotFound();
            }
            return View(inspectItems);
        }

        // POST: InspectItems/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ItemID,AreaID,ClassID,ItemName,ItemStatus")] InspectItems inspectItems)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectItems).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inspectItems);
        }

        // GET: InspectItems/Delete/5
        public ActionResult Delete(int? id, int? Itemid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectItems inspectItems = db.InspectItems.Find(id, Itemid);
            if (inspectItems == null)
            {
                return HttpNotFound();
            }
            return View(inspectItems);
        }

        // POST: InspectItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int Itemid)
        {
            InspectItems inspectItems = db.InspectItems.Find(id, Itemid);
            db.InspectItems.Remove(inspectItems);
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
