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
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            ViewBag.FlowStatusID = new SelectList(db.InspectFlowStatusTable, "FlowStatusID", "FlowStatusName");
            return View();
        }

        // GET: InspectDocSearch/GetData          
        public JsonResult GetData(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
                                  string startDate, string endDate, int? areaId, int? flowStatusId)     //←為表單的查詢條件
        {
            //查詢&排序後的總筆數
            int recordsTotal = 0;

            int FlowStatus = 2;
            try
            {
                var resultList = ((from i in db.InspectDocs
                                   where i.FlowStatusID == FlowStatus
                                   select new
                                   {
                                       AreaName = i.InspectAreas.AreaName,
                                       FlowStatusName = i.InspectFlowStatusTable.FlowStatusName,
                                       Date = i.Date,
                                       WorkerID = i.WorkerID,
                                       WorkerName = i.WorkerName,
                                       CheckerID = i.CheckerID,
                                       CheckerName = i.CheckerName
                                   }).ToList());

                recordsTotal = resultList.Count();//查詢後的總筆數

                //回傳Json資料
                var returnObj =
                    new
                    {
                      draw = draw,
                      recordsTotal = recordsTotal,
                      recordsFiltered = recordsTotal,
                      data = resultList.Skip(start ?? 0).Take(length)   //分頁後的資料 
                    };
                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}