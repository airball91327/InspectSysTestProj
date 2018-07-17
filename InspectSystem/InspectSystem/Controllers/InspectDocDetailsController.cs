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
        public ActionResult Index(int areaID)
        {
            /* Set the DocID to year + month + date + areaID, for example: 2018/10/11 area 1, the docID is 2018101101*/
            string date = DateTime.Now.ToString("yyyyMMdd");
            int docID = System.Convert.ToInt32(date) * 100 + areaID;

            ViewBag.AreaID = areaID;
            ViewBag.AreaName = db.InspectAreas.Find(areaID).AreaName;
            ViewBag.DocID = docID;

            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == areaID)
                                                  .OrderBy(c => c.ClassID);
            return View(ClassesOfAreas.ToList());
        }

        // GET: InspectDocDetails/SelectAreas
        public ActionResult SelectAreas()
        {
            return View(db.InspectAreas.ToList());
        }

        // GET:InspectDocDetails/ClassContentOfArea
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

            /* Create a list for user to insert values, and add some known value first. */
            var inspectDocDetailsTemporary = new List<InspectDocDetailsTemporary>();

            /* Find the temp data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetailsTemp = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID &&
                                                                                 i.ClassID == classID);
            /* If temp data is not found, set values to list. */
            if (inspectDocDetailsTemp.Any() == false)
            {
                foreach (var item in fieldsByACID)
                {
                    Boolean isFunctional = true; // Set default value.
                    var itemName = db.InspectItems.Where(i => i.ItemID == item.ItemID &&
                                                              i.ACID == item.ACID).First();
                    inspectDocDetailsTemporary.Add(new InspectDocDetailsTemporary()
                    {
                        DocID = docID,
                        AreaID = item.ClassesOfAreas.InspectAreas.AreaID,
                        AreaName = item.ClassesOfAreas.InspectAreas.AreaName,
                        ClassID = item.ClassesOfAreas.InspectClasses.ClassID,
                        ClassName = item.ClassesOfAreas.InspectClasses.ClassName,
                        ItemID = item.ItemID,
                        ItemName = itemName.ItemName,
                        FieldID = item.FieldID,
                        FieldName = item.FieldName,
                        UnitOfData = item.UnitOfData,
                        IsFunctional = isFunctional
                    });
                }
            }
            else
            {
                inspectDocDetailsTemporary = inspectDocDetailsTemp.ToList();
            }
            
            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels(){
                    InspectDocDetailsTemporary = inspectDocDetailsTemporary,
                    InspectFields = fieldsByACID,
                    InspectItems = itemsByACID
                };
            /* Return other views for class MedicalGas and UPS with different layout. */
            if(classID == 4 || classID == 5)
            {
                return PartialView("~/Views/InspectDocDetails/ViewOfMedicalGas.cshtml", inspectDocDetailsViewModels);
            }
            else if(classID == 7)
            {
                return PartialView("~/Views/InspectDocDetails/ViewOfUPS.cshtml", inspectDocDetailsViewModels);
            }
            else if(classID == 8)
            {
                int CountItems = inspectDocDetailsViewModels.InspectDocDetailsTemporary.Last().ItemID;
                var MostFields = inspectDocDetailsViewModels.InspectDocDetailsTemporary.Where(i => i.ItemID == 1).ToList();
                for (int j = 2; j <= CountItems; j++)
                {
                     if(inspectDocDetailsViewModels.InspectDocDetailsTemporary.Where(i => i.ItemID == j).Count() > MostFields.Count())
                    {
                        MostFields = inspectDocDetailsViewModels.InspectDocDetailsTemporary.Where(i => i.ItemID == j).ToList();
                    }
                }
                ViewBag.FieldsOfMostItem = MostFields;
                return PartialView("~/Views/InspectDocDetails/ViewOfAirCondition.cshtml", inspectDocDetailsViewModels);
            }
            else
            {
                return PartialView(inspectDocDetailsViewModels);
            }
        }

        // POST:InspectDocDetails/TempSave
        [HttpPost]
        public ActionResult TempSave(List<InspectDocDetailsTemporary> inspectDocDetailsTemporary)
        {
            var areaID = inspectDocDetailsTemporary.First().AreaID;
            var docID = inspectDocDetailsTemporary.First().DocID;
            var classID = inspectDocDetailsTemporary.First().ClassID;

            if (ModelState.IsValid)
            {        
                var findTemp = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID &&
                                                                        i.ClassID == classID);
                /* If can't find temp data, insert data to database. */
                if (findTemp.Any() == false)
                {
                    foreach (var item in inspectDocDetailsTemporary)
                    {
                        db.InspectDocDetailsTemporary.Add(item);
                    }
                }
                else
                {
                    foreach (var item in inspectDocDetailsTemporary)
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                TempData["SaveMsg"] = "暫存完成";
                return RedirectToAction("Index", new { AreaID = areaID });
            }
            TempData["SaveMsg"] = "暫存失敗";
            return RedirectToAction("Index", new { AreaID = areaID });
        }

        // POST:InspectDocDetails/SaveToDataBase
        [HttpPost]
        public ActionResult SaveToDataBase(List<InspectDocDetails> inspectDocDetailsTemporary)
        {
            var areaID = inspectDocDetailsTemporary.First().AreaID;
            var docID = inspectDocDetailsTemporary.First().DocID;
            var classID = inspectDocDetailsTemporary.First().ClassID;

            if (ModelState.IsValid)
            {
                var findDetails = db.InspectDocDetails.Where(i => i.DocID == docID &&
                                                                        i.ClassID == classID);
                /* If can't find temp data, insert data to database. */
                if (findDetails.Any() == false)
                {
                    foreach (var item in inspectDocDetailsTemporary)
                    {
                        db.InspectDocDetails.Add(item);
                    }
                }
                else
                {
                    foreach (var item in inspectDocDetailsTemporary)
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                TempData["SaveMsg"] = "儲存資料完成";
                return RedirectToAction("Index", new { AreaID = areaID });
            }
            TempData["SaveMsg"] = "儲存資料失敗";
            return RedirectToAction("Index", new { AreaID = areaID });
        }

        // GET:InspectDocDetails/AreaPrecautions
        public ActionResult AreaPrecautions(int areaID)
        {
            var areaPrecautions = db.InspectPrecautions.Where(i => i.AreaID == areaID);
            return PartialView(areaPrecautions.ToList());
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
