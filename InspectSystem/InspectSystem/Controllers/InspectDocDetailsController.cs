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

            /* Find the InspectDoc according to the docID, if can't find, new a doc. */
            var FindDoc = db.InspectDocs.Find(docID);
            if (FindDoc == null)
            {
                int userID = 123456;
                string userName = "測試工人";
                int checkerID = 654321;
                string checkerName = "測試主管";

                var inspectDocs = new InspectDocs() {
                    DocID = docID,
                    Date = DateTime.Now,
                    AreaID = areaID,
                    AreaName = db.InspectAreas.Find(areaID).AreaName,
                    UserID = userID,
                    UserName = userName,
                    CheckerID = checkerID,
                    CheckerName = checkerName,
                    FlowStatusID = 0
                };

                db.InspectDocs.Add(inspectDocs);
                db.SaveChanges();
            }
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

        // POST:InspectDocDetails/SaveBeforeSend
        [HttpPost]
        public ActionResult SaveBeforeSend(List<InspectDocDetailsTemporary> inspectDocDetailsTemporary)
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
                return RedirectToAction("DocDetails", new { DocID = docID });
            }
            TempData["SaveMsg"] = "傳送失敗";
            return RedirectToAction("Index", new { AreaID = areaID });
        }

        // GET:InspectDocDetails/AreaPrecautions
        public ActionResult AreaPrecautions(int areaID)
        {
            var areaPrecautions = db.InspectPrecautions.Where(i => i.AreaID == areaID);
            return PartialView(areaPrecautions.ToList());
        }

        //GET:InspectDocDetails/CheckValue
        /* Use ajax to check the min and max value for fields. */
        public ActionResult CheckValue(int AreaID, int ClassID, int ItemID, int FieldID, string Value)
        {
            /* Get the min and max value for the check field. */
            int ACID = db.ClassesOfAreas.Where(i => i.AreaID == AreaID &&
                                                    i.ClassID == ClassID).First().ACID;
            var SearchField = db.InspectFields.Find(ACID, ItemID, FieldID);
            var FieldDataType = SearchField.DataType;
            float MaxValue = System.Convert.ToSingle(SearchField.MaxValue);
            float MinValue = System.Convert.ToSingle(SearchField.MinValue);

            /* Only float type will check. */
            string msg = "";
            if (FieldDataType == "float")
            {
                // Check max and min value, and if doesn't set the min or max value, return nothing.
                if (System.Convert.ToSingle(Value) >= MaxValue && MaxValue != 0)
                {
                    msg = "<span style='color:red'>大於正常數值</span>";
                }
                else if (System.Convert.ToSingle(Value) <= MinValue && MinValue != 0)
                {
                    msg = "<span style='color:red'>小於正常數值</span>";
                }
                else if (MinValue == 0 && MaxValue == 0)
                {
                    msg = "";
                }
                else
                {
                    msg = ""; 
                }
            }
            else
            {
                msg = "";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        // GET: InspectDocDetails/DocDetails
        public ActionResult DocDetails(int docID)
        {
            var DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            int length = docID.ToString().Length;
            int areaID = System.Convert.ToInt32(docID.ToString().Substring(length - 2));

            if (DocDetailList.Count == 0)
            {
                TempData["SaveMsg"] = "尚未有資料儲存";
                return RedirectToAction("Index", new { AreaID = areaID });
            }
            else
            {
                ViewBag.AreaID = DocDetailList.First().AreaID;
                ViewBag.AreaName = DocDetailList.First().AreaName;
                ViewBag.DocID = docID;

                var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == areaID)
                                                      .OrderBy(c => c.ClassID);

                return View(ClassesOfAreas.ToList());
            }
        }

        //POST: InspectDocDetails/SendDocToChecker
        public ActionResult SendDocToChecker(int docID)
        {
            /* Save all temp details to database. */
            var DocDetailTempList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            var areaID = DocDetailTempList.First().AreaID;
            var classID = DocDetailTempList.First().ClassID;
            List<InspectDocDetails> inspectDocDetails = new List<InspectDocDetails>();

            for (int i=0; i<DocDetailTempList.Count(); i++)
            {
                inspectDocDetails[i].DocID = DocDetailTempList[i].DocID;
                inspectDocDetails[i].AreaID = DocDetailTempList[i].AreaID;
                inspectDocDetails[i].AreaName = DocDetailTempList[i].AreaName;
                inspectDocDetails[i].ClassID = DocDetailTempList[i].ClassID;
                inspectDocDetails[i].ClassName = DocDetailTempList[i].ClassName;
                inspectDocDetails[i].ItemID = DocDetailTempList[i].ItemID;
                inspectDocDetails[i].ItemName = DocDetailTempList[i].ItemName;
                inspectDocDetails[i].FieldID = DocDetailTempList[i].FieldID;
                inspectDocDetails[i].FieldName = DocDetailTempList[i].FieldName;
                inspectDocDetails[i].UnitOfData = DocDetailTempList[i].UnitOfData;
                inspectDocDetails[i].Value = DocDetailTempList[i].Value;
                inspectDocDetails[i].IsFunctional = DocDetailTempList[i].IsFunctional;
                inspectDocDetails[i].ErrorDescription = DocDetailTempList[i].ErrorDescription;
                inspectDocDetails[i].RepairDocID = DocDetailTempList[i].RepairDocID;
            }

            if (ModelState.IsValid)
            {
                foreach (var item in inspectDocDetails)
                {
                    db.InspectDocDetails.Add(item);
                }

                db.SaveChanges();
            }
            else
            {
                TempData["SaveMsg"] = "資料儲存失敗";
                return RedirectToAction("Index", new { AreaID = areaID });
            }

            /* Change flow status to "Checking". */
            var findDoc = db.InspectDocs.Find(docID);
            findDoc.FlowStatusID = 1;
            if (ModelState.IsValid)
            {
                db.Entry(findDoc).State = EntityState.Modified;
                db.SaveChanges();
                var msg = "資料已傳送";
                return Json(msg);
            }
            else
            {
                var msg = "資料傳送失敗";
                return Json(msg);
            }
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
