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
            //Initialize the TempData
            if(TempData["AreaListValue"] == null || TempData["ClassListValue"] == null)
            {
                TempData["AreaListValue"] = 0;
                TempData["ClassListValue"] = 0;
            }
            
            //Get the data from table "InspectItems", "InspectAreas", and "InspectClasses"
            var InspectItems = db.InspectItems
                                 .Include(i => i.InspectAreas)
                                 .Include(i => i.InspectClasses);

            //DropDownList for user to select area and class
            ViewBag.AreaList = new SelectList(db.InspectAreas, "AreaID", "AreaName", TempData["AreaListValue"]);
            ViewBag.ClassList = new SelectList(db.InspectClasses, "ClassID", "ClassName", TempData["ClassListValue"]);

            return View(InspectItems.ToList());
        }

        // GET: InspectItems/SearchItems
        /* Get the area and class that user selected, and return search result. */
        public ActionResult SearchItems()
        {
            int AreaListValue = System.Convert.ToInt32(Request.Form["AreaList"]);
            int ClassListValue = System.Convert.ToInt32(Request.Form["ClassList"]);
            TempData["AreaListValue"] = AreaListValue;
            TempData["ClassListValue"] = ClassListValue;

            var InspectItems = db.InspectItems
                                 .Include(i => i.InspectAreas)
                                 .Include(i => i.InspectClasses);
            var SearchResult = InspectItems
                               .Where(s => s.AreaID == AreaListValue &&
                                           s.ClassID == ClassListValue);
            TempData["SearchResult"]  = SearchResult.ToList(); 

            return RedirectToAction("Index");
        }

        // GET: InspectItems/ReloadPage
        /* After create items or edit items, will redirect to the function to reload page. */
        public ActionResult ReloadPage()
        {
            //將保留的搜尋資訊，重新去搜尋更新後的資料
            int AreaListValue = System.Convert.ToInt32(TempData["AreaListValue"]);
            int ClassListValue = System.Convert.ToInt32(TempData["ClassListValue"]);

            var InspectItems = db.InspectItems
                                 .Include(i => i.InspectAreas)
                                 .Include(i => i.InspectClasses);
            var SearchResult = InspectItems
                               .Where(s => s.AreaID == AreaListValue &&
                                           s.ClassID == ClassListValue);
            TempData["SearchResult"] = SearchResult.ToList();

            return RedirectToAction("Index");
        }

        /* Unused code
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
        */

        // POST: InspectItems/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Create(InspectItems inspectItems)
        {

            Boolean itemStatus = true;

            //Get the value from Request.
            int areaID = System.Convert.ToInt32(Request.Form["areaID"]);
            int classID = System.Convert.ToInt32(Request.Form["classID"]);
            int ACID = (areaID) * 100 + classID;
            int itemID = System.Convert.ToInt32(Request.Form["itemID"]);

            /* If CheckBox is not selected, it will return nothing, 
               so use the condition to give value to checkbox's request. */
            if (Request.Form["itemStatus"] != null)
            {
                itemStatus = true;
            }
            else
                itemStatus = false;

            //Insert the values to create items
            inspectItems.ACID = ACID;
            inspectItems.AreaID = areaID;
            inspectItems.ClassID = classID;
            inspectItems.ItemID = itemID;
            inspectItems.ItemName = Request.Form["itemName"];
            inspectItems.ItemStatus = itemStatus;

            /* If can't find the data from ClassesOfAreas table, insert a new data in the table. */
            ClassesOfAreas classesOfAreas = db.ClassesOfAreas.Find(ACID);
            if(classesOfAreas == null)
            {
                classesOfAreas.ACID = ACID;
                classesOfAreas.AreaID = areaID;
                classesOfAreas.ClassID = classID;
                db.ClassesOfAreas.Add(classesOfAreas);
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                db.InspectItems.Add(inspectItems);
                db.SaveChanges();
                return RedirectToAction("ReloadPage");
            }

            return RedirectToAction("ReloadPage");
        }

        // POST: InspectItems/Edit
        [HttpPost]
        public ActionResult Edit()
        {
            Boolean itemStatus = true;

            //找出要更改數值的資料
            int ACID = System.Convert.ToInt32(Request.Form["item.ACID"]);
            int itemID = System.Convert.ToInt32(Request.Form["item.ItemID"]);
            InspectItems inspectItems = db.InspectItems.Find(ACID, itemID);

            //處理Request.Form無法處理Checkbox回傳值的問題
            if( Request.Form["item.ItemStatus"].Contains("true") == true )
            {
                itemStatus = true;
            }
            else
                itemStatus = false;

            //更改ItemName跟ItemStatus的數值
            inspectItems.ItemName = Request.Form["item.ItemName"];
            inspectItems.ItemStatus = itemStatus;

            if (ModelState.IsValid)
            {
                db.Entry(inspectItems).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ReloadPage");
            }
            return RedirectToAction("ReloadPage");
        }

        /* Unused code
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
