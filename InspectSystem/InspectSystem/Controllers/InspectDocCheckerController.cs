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
    public class InspectDocCheckerController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        public ActionResult Index()
        {
            return View();
        }

        // GET: InspectDocChecker/DocListForChecker
        public ActionResult DocListForChecker(int UserID)
        {
            var CheckingDocs = db.InspectDocs.Where(i => i.CheckerID == UserID &&
                                                         i.FlowStatusID == 1);
            TempData["UserID"] = UserID;
            TempData["Role"] = "Checker";
            TempData.Keep("UserID");
            TempData.Keep("Role");
            return View(CheckingDocs.ToList());
        }

        // GET: InspectDocChecker/DocListForWorker
        public ActionResult DocListForWorker(int UserID)
        {
            var CheckingDocs = db.InspectDocs.Where(i => i.WorkerID == UserID &&
                                                         i.FlowStatusID == 0);
            TempData["UserID"] = UserID;
            TempData["Role"] = "Worker";
            TempData.Keep("UserID");
            TempData.Keep("Role");
            return View(CheckingDocs.ToList());
        }

        // GET: InspectDocChecker/DocDetails
        public ActionResult DocDetails(int docID)
        {
            /* Set variables from DB. */
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            int length = docID.ToString().Length;
            int areaID = System.Convert.ToInt32(docID.ToString().Substring(length - 2));

            ViewBag.AreaID = DocDetailList.First().AreaID;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocID = docID;

            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == areaID)
                                                  .OrderBy(c => c.ClassID);
            return View(ClassesOfAreas.ToList());
        }

        // GET:InspectDocChecker/ClassContentOfArea
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

            /* Find the data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails.ToList(),
                InspectFields = fieldsByACID,
                InspectItems = itemsByACID
            };

            /* Return other views for class MedicalGas and UPS with different layout. */
            if (classID == 4 || classID == 5)
            {
                return PartialView("~/Views/InspectDocChecker/ViewOfMedicalGas.cshtml", inspectDocDetailsViewModels);
            }
            else if (classID == 7)
            {
                return PartialView("~/Views/InspectDocChecker/ViewOfUPS.cshtml", inspectDocDetailsViewModels);
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
                return PartialView("~/Views/InspectDocChecker/ViewOfAirCondition.cshtml", inspectDocDetailsViewModels);
            }
            else
            {
                return PartialView(inspectDocDetailsViewModels);
            }
        }

        // GET:InspectDocChecker/GetFlowList
        public ActionResult GetFlowList(int docID)
        {
            var flowList = db.InspectDocFlows.Where(i => i.DocID == docID).OrderBy(i => i.StepID);
            var findDoc = db.InspectDocs.Find(docID);

            foreach( var item in flowList)
            {
                if(item.StepOwnerID == item.WorkerID)
                {
                    item.StepOwnerName = findDoc.WorkerName;
                }
                else if(item.StepOwnerID == item.CheckerID)
                {
                    item.StepOwnerName = findDoc.CheckerName;
                }
            }

            return PartialView(flowList.ToList());
        }

        // GET:InspectDocChecker/FlowDoc
        public ActionResult FlowDoc(int docID, int userID, string role)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocID == docID)
                                            .OrderByDescending(i => i.StepID).First();

            /* Use userID to find the user details. (Not Implement)*/
            flowDoc.EditorID = userID;
            flowDoc.EditorName = "資料庫撈userName";
            flowDoc.Opinions = "";

            /* According user role to retrun views. */
            if( role == "Checker" )
            {
                return PartialView("FlowDocEditForChecker", flowDoc);
            }
            else 
            {
                return PartialView("FlowDocEditForWorker", flowDoc);
            }
        }

        // POST:InspectDocChecker/FlowDocEditForChecker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlowDocEditForChecker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorID;
            int docID = inspectDocFlow.DocID;
            var inspectDoc = db.InspectDocs.Find(docID);
            int nextFlowStatusID = System.Convert.ToInt32(Request.Form["NextFlowStatusID"]);

            /* Insert edit time, and change flow status for inspect doc. */
            inspectDocFlow.EditTime = DateTime.Now;
            inspectDoc.FlowStatusID = nextFlowStatusID;

            /* If doc is send back to worker. */
            if(nextFlowStatusID == 0)
            {
                /* New next flow for worker. */
                InspectDocFlow nextDocFlow = new InspectDocFlow()
                {
                    DocID = docID,
                    StepID = inspectDocFlow.StepID + 1,
                    StepOwnerID = inspectDocFlow.WorkerID,
                    WorkerID = inspectDocFlow.WorkerID,
                    CheckerID = inspectDocFlow.CheckerID,
                    Opinions = "",
                    FlowStatusID = nextFlowStatusID,
                    EditorID = 0,
                    EditorName = "",
                    EditTime = null,
                };

                if (ModelState.IsValid)
                {
                    db.InspectDocFlows.Add(nextDocFlow);
                    db.SaveChanges();
                }
            }

            if(ModelState.IsValid)
            {
                /* Add new data to doc flow, and modefiy doc. */
                db.Entry(inspectDocFlow).State = EntityState.Modified;
                db.Entry(inspectDoc).State = EntityState.Modified;
                db.SaveChanges();

                /* return save success message. */
                if(inspectDocFlow.FlowStatusID == 2)
                {
                    TempData["SendMsg"] = "文件已結案";
                }
                else if(inspectDocFlow.FlowStatusID == 0)
                {
                    TempData["SendMsg"] = "文件已退回";
                }
                return RedirectToAction("DocListForChecker", new { UserID = userID });
            }
            else
            {
                TempData["SendMsg"] = "文件傳送失敗";
                return RedirectToAction("DocListForChecker", new { UserID = userID });
            }
        }

        // POST:InspectDocChecker/FlowDocEditForWorker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlowDocEditForWorker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorID;
            int docID = inspectDocFlow.DocID;
            var inspectDoc = db.InspectDocs.Find(docID);

            /* Insert edit time, and change flow status to "checking" for doc. */
            inspectDocFlow.EditTime = DateTime.Now;
            inspectDoc.FlowStatusID = 1;

            /* New next flow for checker. */
            InspectDocFlow nextDocFlow = new InspectDocFlow()
            {
                DocID = docID,
                StepID = inspectDocFlow.StepID + 1,
                StepOwnerID = inspectDocFlow.CheckerID,
                WorkerID = inspectDocFlow.WorkerID,
                CheckerID = inspectDocFlow.CheckerID,
                Opinions = "",
                FlowStatusID = 1,
                EditorID = 0,
                EditorName = "",
                EditTime = null,
            };

            if (ModelState.IsValid)
            {
                db.Entry(inspectDocFlow).State = EntityState.Modified;
                db.InspectDocFlows.Add(nextDocFlow);
                db.Entry(inspectDoc).State = EntityState.Modified;
                db.SaveChanges();

                /* return save success message. */
                TempData["SendMsg"] = "文件傳送成功";
                return RedirectToAction("DocListForWorker", new { UserID = userID });
            }
            else
            {
                TempData["SendMsg"] = "文件傳送失敗";
                return RedirectToAction("DocListForWorker", new { UserID = userID });
            }
        }
    }
}