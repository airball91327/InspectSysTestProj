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

namespace InspectSystem.Areas.Mobile.Controllers
{
    [Authorize]
    public class InspectDocCheckerController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        public ActionResult Index()
        {
            /* Get current user. */
            //var userId = User.Identity.GetUserId();
            var userName = WebSecurity.CurrentUserName;

            //if (User.IsInRole("Admin") == true)
            //{
            //    return RedirectToAction("DocListForChecker", "InspectDocChecker", new { Area = "Mobile" });
            //}
            //else if (User.IsInRole("Supervisor") == true)
            //{
            //    return RedirectToAction("DocListForChecker", "InspectDocChecker", new { Area = "Mobile" });
            //}
            //else
            //{
            //    return RedirectToAction("DocListForWorker", "InspectDocChecker", new { Area = "Mobile" });
            //}

            // For testing
            if (userName == "344027")
            {
                return RedirectToAction("DocListForChecker", "InspectDocChecker", new { Area = "Mobile" });
            }
            else
            {
                return RedirectToAction("DocListForWorker", "InspectDocChecker", new { Area = "Mobile" });
            }
        }

        // GET: Mobile/InspectDocChecker/DocListForChecker
        public ActionResult DocListForChecker()
        {
            int UserID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            var CheckingDocs = db.InspectDocs.Where(i => i.CheckerID == UserID &&
                                                         i.FlowStatusID == 1);
          
            return View(CheckingDocs.ToList());
        }

        // GET: Mobile/InspectDocChecker/DocListForWorker
        public ActionResult DocListForWorker()
        {
            int UserID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            var CheckingDocs = db.InspectDocs.Where(i => i.WorkerID == UserID &&
                                                         i.FlowStatusID == 0);
            
            return View(CheckingDocs.ToList());
        }

        // GET: Mobile/InspectDocChecker/DocDetails
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

        // GET: Mobile/InspectDocChecker/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.DocID = docID;

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

            return View(inspectDocDetailsViewModels);
        }

        // GET: Mobile/InspectDocChecker/GetFlowList
        public ActionResult GetFlowList(int docID)
        {
            var flowList = db.InspectDocFlows.Where(i => i.DocID == docID).OrderByDescending(i => i.StepID);
            var findDoc = db.InspectDocs.Find(docID);

            foreach (var item in flowList)
            {
                if (item.StepOwnerID == item.WorkerID)
                {
                    item.StepOwnerName = findDoc.WorkerName;
                }
                else if (item.StepOwnerID == item.CheckerID)
                {
                    item.StepOwnerName = findDoc.CheckerName;
                }
            }

            return View(flowList.ToList());
        }

        // GET: Mobile/InspectDocChecker/FlowDoc
        public ActionResult FlowDoc(int docID)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocID == docID)
                                            .OrderByDescending(i => i.StepID).First();

            ViewBag.AreaID = flowDoc.InspectDocs.AreaID;

            /* Use userID to find the user details. (Not Implement)*/
            flowDoc.EditorID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            flowDoc.EditorName = "資料庫撈userName";
            flowDoc.Opinions = "";

            /* According user role to retrun views. */
            //if (User.IsInRole("Supervisor") == true)
            //{
            //    return View("FlowDocEditForChecker", flowDoc);
            //}
            //else
            //{
            //    return View("FlowDocEditForWorker", flowDoc);
            //}

            // For testing
            if (WebSecurity.CurrentUserName == "344027")
            {
                return View("FlowDocEditForChecker", flowDoc);
            }
            else
            {
                return View("FlowDocEditForWorker", flowDoc);
            }
        }

        // POST: Mobile/InspectDocChecker/FlowDocEditForChecker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlowDocEditForChecker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorID;
            int docID = inspectDocFlow.DocID;
            var inspectDoc = db.InspectDocs.Find(docID);
            int nextFlowStatusID = System.Convert.ToInt32(Request.Form["NextFlowStatusID"]);

            /* Insert edit time, and change flow status for inspect doc. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
            inspectDoc.FlowStatusID = nextFlowStatusID;

            /* If doc is send back to worker. */
            if (nextFlowStatusID == 0)
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

            if (ModelState.IsValid)
            {
                /* Add new data to doc flow, and modefiy doc. */
                db.Entry(inspectDocFlow).State = EntityState.Modified;
                db.Entry(inspectDoc).State = EntityState.Modified;
                db.SaveChanges();

                /* return save success message. */
                if (inspectDocFlow.FlowStatusID == 2)
                {
                    TempData["SendMsg"] = "文件已結案";
                }
                else if (inspectDocFlow.FlowStatusID == 0)
                {
                    TempData["SendMsg"] = "文件已退回";
                }
                return RedirectToAction("DocListForChecker", new { area = "Mobile" });
            }
            else
            {
                TempData["SendMsg"] = "文件傳送失敗";
                return RedirectToAction("DocListForChecker", new { area = "Mobile" });
            }
        }

        // POST: Mobile/InspectDocChecker/FlowDocEditForWorker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlowDocEditForWorker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorID;
            int docID = inspectDocFlow.DocID;
            var inspectDoc = db.InspectDocs.Find(docID);

            /* Insert edit time, and change flow status to "checking" for doc. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
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
                return RedirectToAction("DocListForWorker", new { area = "Mobile" });
            }
            else
            {
                TempData["SendMsg"] = "文件傳送失敗";
                return RedirectToAction("DocListForWorker", new { area = "Mobile" });
            }
        }
    }
}