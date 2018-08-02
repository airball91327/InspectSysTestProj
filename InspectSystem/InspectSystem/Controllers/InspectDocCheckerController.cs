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
        public ActionResult DocListForChecker(string userID)
        {
            int UserID = System.Convert.ToInt32(userID);
            var CheckingDocs = db.InspectDocs.Where(i => i.CheckerID == UserID &&
                                                         i.FlowStatusID == 1);
            TempData["UserID"] = UserID;
            TempData["Role"] = "Checker";
            return View(CheckingDocs.ToList());
        }

        // GET: InspectDocChecker/DocListForWorker
        public ActionResult DocListForWorker(string userID)
        {
            int UserID = System.Convert.ToInt32(userID);
            var CheckingDocs = db.InspectDocs.Where(i => i.WorkerID == UserID &&
                                                         i.FlowStatusID == 0);
            TempData["UserID"] = UserID;
            TempData["Role"] = "Worker";
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

            return PartialView(flowList.ToList());
        }

        // GET:InspectDocChecker/FlowDoc
        public ActionResult FlowDoc(int docID, int userID, string role)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocID == docID)
                                            .OrderByDescending(i => i.StepID).First();
            flowDoc.StepID++;

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
        public ActionResult FlowDocEditForChecker(InspectDocFlow inspectDocFlow)
        {
            //not implement
            return View();
        }

        // POST:InspectDocChecker/FlowDocEditForWorker
        public ActionResult FlowDocEditForWorker(InspectDocFlow inspectDocFlow)
        {
            //not implement
            return View();
        }
    }
}