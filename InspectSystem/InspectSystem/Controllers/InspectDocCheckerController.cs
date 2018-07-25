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

        // GET: InspectDocChecker
        public ActionResult Index()
        {
            return View();
        }

        // GET: InspectDocChecker/DocDetails
        public ActionResult DocDetails(int docID)
        {
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();

            ViewBag.AreaID = DocDetailList.First().AreaID;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocID = docID;

            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == DocDetailList.First().AreaID)
                                                  .OrderBy(c => c.ClassID);

            return View(ClassesOfAreas.ToList());
        }

        // GET: InspectDocChecker/DocDetails2
        public ActionResult DocDetails2(int docID)
        {
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();

            ViewBag.AreaID = DocDetailList.First().AreaID;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocID = docID;

            var ClassesOfAreas = db.ClassesOfAreas.Where(c => c.AreaID == DocDetailList.First().AreaID)
                                                  .OrderBy(c => c.ClassID);

            return View(ClassesOfAreas.ToList());
        }
    }
}