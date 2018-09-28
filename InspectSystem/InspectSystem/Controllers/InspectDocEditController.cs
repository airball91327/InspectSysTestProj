using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocEditController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocEdit
        public ActionResult Index(int docID)
        {
            var theEditDoc = db.InspectDocs.Find(docID);
            int areaID = theEditDoc.AreaID;
            ViewBag.AreaID = areaID;
            ViewBag.AreaName = theEditDoc.AreaName;
            ViewBag.DocID = docID;
            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == areaID)
                                                  .OrderBy(c => c.InspectClasses.ClassOrder);
            /* Count errors for every class, and set count result to "CountErrors". */
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            foreach (var item in ClassesOfAreas)
            {
                var toFindErrors = DocDetailList.Where(d => d.ClassID == item.ClassID &&
                                                           d.IsFunctional == false);
                item.CountErrors = toFindErrors.Count();
            }
            return View(ClassesOfAreas.ToList());
        }

        // GET: InspectDocEdit/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;

            /* Get items and fields to display. */
            var inspectFields = db.InspectFields.Include(i => i.ClassesOfAreas)
                                                .Include(i => i.ClassesOfAreas.InspectAreas)
                                                .Include(i => i.ClassesOfAreas.InspectClasses);
            var itemsByACID = db.InspectItems.Where(i => i.ACID == ACID &&
                                                         i.ItemStatus == true).ToList();
            var fieldsByACID = inspectFields.Where(i => i.ACID == ACID &&
                                                        i.FieldStatus == true).ToList();
            var fieldDropDown = db.InspectFieldDropDown.Where(i => i.ACID == ACID).ToList();

            /* Find the doc details. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails,
                InspectFields = fieldsByACID,
                InspectItems = itemsByACID,
                InspectFieldDropDowns = fieldDropDown
            };

            return PartialView(inspectDocDetailsViewModels);
        }

        // POST: InspectDocEdit/SaveData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveData(List<InspectDocDetails> inspectDocDetails)
        {
            var areaID = inspectDocDetails.First().AreaID;
            int docID = inspectDocDetails.First().DocID;

            if (ModelState.IsValid)
            {
                foreach (var item in inspectDocDetails)
                {
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
                TempData["SaveMsg"] = "資料修改完成";
                return RedirectToAction("Index", new { DocID = docID });
            }
            TempData["SaveMsg"] = "資料修改失敗";
            return RedirectToAction("Index", new { DocID = docID });
        }

        /*
        // POST: InspectDocEdit/SaveBeforeSend
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBeforeSend(List<InspectDocDetails> inspectDocDetails)
        {
            var areaID = inspectDocDetails.First().AreaID;
            int docID = inspectDocDetails.First().DocID;

            if (ModelState.IsValid)
            {
                foreach (var item in inspectDocDetails)
                {
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("DocDetails", new { DocID = docID, AreaID = areaID });
            }
            TempData["SaveMsg"] = "傳送失敗";
            return RedirectToAction("Index", new { DocID = docID });
        }
        */

        // GET: InspectDocDetails/DocDetails
        public ActionResult DocDetails(int docID)
        {
            var theEditDoc = db.InspectDocs.Find(docID);
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            int areaID = theEditDoc.AreaID;

            ViewBag.AreaID = areaID;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocID = docID;

            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == areaID)
                                                  .OrderBy(c => c.InspectClasses.ClassOrder);
            /* Count errors for every class, and set count result to "CountErrors". */
            foreach (var item in ClassesOfAreas)
            {
                var toFindErrors = DocDetailList.Where(d => d.ClassID == item.ClassID &&
                                                           d.IsFunctional == false);
                item.CountErrors = toFindErrors.Count();
            }
            return View(ClassesOfAreas.ToList());
        }

    }
}