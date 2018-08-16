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
                int workerID = 123456;
                string workerName = "測試工人";

                /* Find the checker of the area. */
                int checkerID = 0;
                string checkerName = "";
                var findAreaChecker = db.inspectAreaCheckers.Find(areaID);
                if(findAreaChecker != null)
                {
                    checkerID = findAreaChecker.CheckerID;
                    checkerName = findAreaChecker.CheckerName;
                }

                var inspectDocs = new InspectDocs() {
                    DocID = docID,
                    Date = DateTime.Now,
                    AreaID = areaID,
                    AreaName = db.InspectAreas.Find(areaID).AreaName,
                    WorkerID = workerID,
                    WorkerName = workerName,
                    CheckerID = checkerID,
                    CheckerName = checkerName,
                    FlowStatusID = 3        // Default flow status:"編輯中"
                };

                db.InspectDocs.Add(inspectDocs);
                db.SaveChanges();
            }
            TempData["UserID"] = db.InspectDocs.Find(docID).WorkerID;
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
            /* Return views with different layout. */
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

        // GET: InspectDocDetails/AreaPrecautions
        public ActionResult AreaPrecautions(int areaID)
        {
            var areaPrecautions = db.InspectPrecautions.Where(i => i.AreaID == areaID);
            return PartialView(areaPrecautions.ToList());
        }

        //GET: InspectDocDetails/CheckValue
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
                /* Check the input string can be convert to float. */
                if (Single.TryParse(Value, out float inputValue))
                {
                    // Check max and min value, and if doesn't set the min or max value, return nothing.
                    if (inputValue >= MaxValue && MaxValue != 0)
                    {
                        msg = "<span style='color:red'>大於正常數值</span>";
                    }
                    else if (inputValue <= MinValue && MinValue != 0)
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
                    msg = "<span style='color:red'>請輸入數字</span>";
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

        // GET: InspectDocDetails/FlowDocEdit
        public ActionResult FlowDocEdit(int docID, int userID)
        {

            var findDoc = db.InspectDocs.Find(docID);
            int areaID = findDoc.AreaID;
            int checkerID = db.inspectAreaCheckers.Find(areaID).CheckerID;
            var areaCheckers = db.inspectAreaCheckers.ToList();
            var areaCheckerNames = areaCheckers.GroupBy(a => a.CheckerName).Select(g => g.First()).ToList();

            SelectList areaCheckerSelectList = new SelectList(areaCheckerNames, "CheckerID", "CheckerName", checkerID);
            ViewBag.AreaCheckerNames = areaCheckerSelectList;

            /* Set value to new first DocFlow. */
            InspectDocFlow DocFlow = new InspectDocFlow()
            {
                DocID = docID,
                StepID = 1,
                StepOwnerID = findDoc.WorkerID,
                WorkerID = findDoc.WorkerID,
                CheckerID = findDoc.CheckerID,
                Opinions = "",
                FlowStatusID = 0,
                EditorID = findDoc.WorkerID,
                EditorName = findDoc.WorkerName,
                EditTime = null,
                InspectDocs = db.InspectDocs.Find(docID)
            };

            return PartialView(DocFlow);
        }

        // POST: InspectDocDetails/SendDocToChecker
        public ActionResult SendDocToChecker(InspectDocFlow inspectDocFlow)
        {
            /* Declear variables, and search data from DB. */
            int docID = inspectDocFlow.DocID;
            var DocDetailTempList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            var areaID = DocDetailTempList.First().AreaID;
            var classID = DocDetailTempList.First().ClassID;
            List<InspectDocDetails> inspectDocDetails = new List<InspectDocDetails>();
            var findDocDetails = db.InspectDocDetails.Where(i => i.DocID == docID);
            var findDoc = db.InspectDocs.Find(docID);
            var checkerID = System.Convert.ToInt32(Request.Form["AreaCheckerNames"]);

            /* Save all temp details to database. */
            /* Copy temp data to inspectDocDetails list. */
            foreach (var item in DocDetailTempList)
            {
                inspectDocDetails.Add(new InspectDocDetails()
                {
                    DocID = item.DocID,
                    AreaID = item.AreaID,
                    AreaName = item.AreaName,
                    ClassID = item.ClassID,
                    ClassName = item.ClassName,
                    ItemID = item.ItemID,
                    ItemName = item.ItemName,
                    FieldID = item.FieldID,
                    FieldName = item.FieldName,
                    UnitOfData = item.UnitOfData,
                    Value = item.Value,
                    IsFunctional = item.IsFunctional,
                    ErrorDescription = item.ErrorDescription,
                    RepairDocID = item.RepairDocID
                });
            }
            
            if (ModelState.IsValid)
            {
                if(findDocDetails.Count() == 0)
                {
                    foreach (var item in inspectDocDetails)
                    {
                        db.InspectDocDetails.Add(item);
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (var item in inspectDocDetails)
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                TempData["SaveMsg"] = "資料儲存失敗";
                return RedirectToAction("Index", new { AreaID = areaID });
            }

            /* Change flow status to "Checking" for this doc. */
            findDoc.FlowStatusID = 1;
            findDoc.EndTime = DateTime.Now;
            findDoc.CheckerID = checkerID;
            findDoc.CheckerName = db.inspectAreaCheckers.Where(i => i.CheckerID == checkerID).First().CheckerName;

            /* Set edit time and checkerID for doc flow. */
            inspectDocFlow.EditTime = DateTime.Now;
            inspectDocFlow.CheckerID = checkerID;

            /* The next flow for checker. */
            InspectDocFlow NextDocFlow = new InspectDocFlow()
            {
                DocID = docID,
                StepID = inspectDocFlow.StepID + 1,
                StepOwnerID = findDoc.CheckerID,
                WorkerID = findDoc.WorkerID,
                CheckerID = findDoc.CheckerID,
                Opinions = "",
                FlowStatusID = 1,
                EditorID = 0,
                EditorName = "",
                EditTime = null,
            };

            if (ModelState.IsValid)
            {
                db.InspectDocFlows.Add(inspectDocFlow);
                db.InspectDocFlows.Add(NextDocFlow);
                db.Entry(findDoc).State = EntityState.Modified;
                db.SaveChanges();
                var msg = "資料已傳送";
                return RedirectToAction("AfterSendDoc", new { Msg = msg });
            }
            else
            {
                TempData["SaveMsg"] = "資料傳送失敗";
                return RedirectToAction("Index", new { AreaID = areaID });
            }
        }

        // GET: InspectDocDetails/AfterSendDoc
        public ActionResult AfterSendDoc(string Msg)
        {
            ViewBag.msg = Msg;
            return View();
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
