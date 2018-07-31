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
    public class InspectDocTempViewController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocTempView
        public ActionResult Index()
        {
            return View();
        }

        // GET:InspectDocTempView/ClassContentOfArea
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
            var inspectDocDetailsTemp = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetailsTemporary = inspectDocDetailsTemp.ToList(),
                InspectFields = fieldsByACID,
                InspectItems = itemsByACID
            };

            /* Return other views for class MedicalGas and UPS with different layout. */
            if (classID == 4 || classID == 5)
            {
                return PartialView("~/Views/InspectDocTempView/ViewOfMedicalGas.cshtml", inspectDocDetailsViewModels);
            }
            else if (classID == 7)
            {
                return PartialView("~/Views/InspectDocTempView/ViewOfUPS.cshtml", inspectDocDetailsViewModels);
            }
            else if (classID == 8)
            {
                int CountItems = inspectDocDetailsViewModels.InspectDocDetailsTemporary.Last().ItemID;
                var MostFields = inspectDocDetailsViewModels.InspectDocDetailsTemporary.Where(i => i.ItemID == 1).ToList();
                for (int j = 2; j <= CountItems; j++)
                {
                    if (inspectDocDetailsViewModels.InspectDocDetailsTemporary.Where(i => i.ItemID == j).Count() > MostFields.Count())
                    {
                        MostFields = inspectDocDetailsViewModels.InspectDocDetailsTemporary.Where(i => i.ItemID == j).ToList();
                    }
                }
                ViewBag.FieldsOfMostItem = MostFields;
                return PartialView("~/Views/InspectDocTempView/ViewOfAirCondition.cshtml", inspectDocDetailsViewModels);
            }
            else
            {
                return PartialView(inspectDocDetailsViewModels);
            }
        }
    }
}