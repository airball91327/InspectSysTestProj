﻿using System;
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
                                                  .OrderBy(c => c.ClassID);

            TempData["UserID"] = theEditDoc.WorkerID;
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

            /* Find the doc details. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails,
                InspectFields = fieldsByACID,
                InspectItems = itemsByACID
            };

            /* Return other views for class MedicalGas and UPS with different layout. */
            if (classID == 4 || classID == 5)
            {
                return PartialView("~/Views/InspectDocEdit/ViewOfMedicalGas.cshtml", inspectDocDetailsViewModels);
            }
            else if (classID == 7)
            {
                return PartialView("~/Views/InspectDocEdit/ViewOfUPS.cshtml", inspectDocDetailsViewModels);
            }
            else if (classID == 8)
            {
                int CountItems = inspectDocDetailsViewModels.InspectDocDetails.Last().ItemID;
                var MostFields = inspectDocDetailsViewModels.InspectDocDetails.Where(i => i.ItemID == 1).ToList();
                for (int j = 2; j <= CountItems; j++)
                {
                    if (inspectDocDetailsViewModels.InspectDocDetails.Where(i => i.ItemID == j).Count() > MostFields.Count())
                    {
                        MostFields = inspectDocDetailsViewModels.InspectDocDetails.Where(i => i.ItemID == j).ToList();
                    }
                }
                ViewBag.FieldsOfMostItem = MostFields;
                return PartialView("~/Views/InspectDocEdit/ViewOfAirCondition.cshtml", inspectDocDetailsViewModels);
            }
            else
            {
                return PartialView(inspectDocDetailsViewModels);
            }
        }

        // POST: InspectDocEdit/SaveData
        [HttpPost]
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

        // POST: InspectDocEdit/SaveBeforeSend
        [HttpPost]
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

        // GET: InspectDocDetails/DocDetails
        public ActionResult DocDetails(int docID, int areaID)
        {
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();

            ViewBag.AreaID = DocDetailList.First().AreaID;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocID = docID;

            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == areaID)
                                                  .OrderBy(c => c.ClassID);

            return View(ClassesOfAreas.ToList());
        }

    }
}