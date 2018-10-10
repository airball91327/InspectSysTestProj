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
    [Authorize]
    public class InspectDocSearchController : Controller
    {

        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocSearch
        public ActionResult Index()
        {
            return View();
        }

        // GET: InspectDocSearch/GetData
        public JsonResult GetData()
        {
            int FlowStatus = 2;
            try
            {
                var resultList = ((from i in db.InspectDocs
                                   where i.FlowStatusID == FlowStatus
                                   select new
                                   {
                                       AreaName = i.InspectAreas.AreaName,
                                       FlowStatusName = i.FlowStatusID,
                                       Date = i.Date,
                                       WorkerID = i.WorkerID,
                                       WorkerName = i.WorkerName,
                                       CheckerID = i.CheckerID,
                                       CheckerName = i.CheckerName
                                   }).ToList());
                return Json(resultList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}