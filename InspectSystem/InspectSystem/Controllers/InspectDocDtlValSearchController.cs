using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocDtlValSearchController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocDtlValSearch
        public ActionResult Index()
        {
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            return View();
        }

        // GET: InspectDocDtlValSearch/GetData          
        public JsonResult GetData(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
                                  DateTime startDate, DateTime endDate, int areaId, int classId,
                                  int itemId, string fieldSearchText)
                                  //↑為表單的查詢條件
        {
            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //jQuery DataTable的Column index
            string col_index = Request.QueryString["order[0][column]"];
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "Date" : Request.QueryString[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(Request.QueryString["order[0][dir]"]) ? "asc" : Request.QueryString["order[0][dir]"];//防呆

            try
            {
                /* 查詢日期 */
                int fromDate = System.Convert.ToInt32(startDate.ToString("yyyyMMdd"));
                int toDate = System.Convert.ToInt32(endDate.ToString("yyyyMMdd"));
                int fromDoc, toDoc;     // Set doc search range.
                if (fromDate > toDate)
                {
                    fromDoc = (toDate * 100) + 1;
                    toDoc = (fromDate * 100) + 99;
                }
                else
                {
                    fromDoc = (fromDate * 100) + 1;
                    toDoc = (toDate * 100) + 99;
                }
                var searchList = db.InspectDocDetails.Where(i => i.DocID >= fromDoc && i.DocID <= toDoc);

                /* 查詢區域、類別 */
                searchList = searchList.Where(s => s.AreaID == areaId &&
                                                   s.ClassID == classId);
                /* 查詢類別 */
                if (itemId != 0)
                {
                    searchList = searchList.Where(s => s.ItemID == itemId);
                }

                /* 查詢欄位關鍵字 */
                if (fieldSearchText != "")
                {
                    searchList = searchList.Where(s => s.FieldName.Contains(fieldSearchText));
                }               

                /* 處理儲存正常或不正常的欄位，把Value拿來顯示是否正常. */
                foreach(var item in searchList)
                {
                    if( item.DataType == "boolean" )
                    {
                        if( item.IsFunctional == "y" )
                        {
                            item.Value = "正常";
                        }
                        else
                        {
                            item.Value = "不正常";
                        }
                    }
                }

                var resultList = searchList.AsEnumerable().Select(s => new
                {
                    Date = s.InspectDocs.Date.ToString("yyyy/MM/dd"),   // ToString() is not supported in Linq to Entities, 
                    AreaName = s.AreaName,                              // need to change type to IEnumerable by using AsEnumerable(),
                    ClassName = s.ClassName,                            // and then can use ToString(), 
                    ItemName = s.ItemName,                              // because AsEnumerable() is Linq to Objects.
                    FieldName = s.FieldName,
                    Value = s.Value,
                    UnitOfData = s.UnitOfData,
                    DocID = s.DocID,
                    AreaID = s.AreaID
                }).ToList();

                // Deal DataTable sorting. 
                resultList = resultList.AsEnumerable().OrderBy($@"{sortColName} {asc_desc}").ToList();

                recordsTotal = resultList.Count();//查詢後的總筆數

                // 分頁處理
                if(length != -1) //-1為顯示所有資料
                {
                    resultList = resultList.Skip(start ?? 0).Take(length).ToList();   //分頁後的資料 
                }

                //回傳Json資料
                var returnObj =
                    new
                    {
                        draw = draw,
                        recordsTotal = recordsTotal,
                        recordsFiltered = recordsTotal,
                        data = resultList
                    };
                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: InspectDocDtlValSearch/GetClasses
        [HttpPost]
        public JsonResult GetClasses(int AreaID)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            db.ClassesOfAreas.Where(c => c.AreaID == AreaID)
                             .OrderBy(c => c.InspectClasses.ClassOrder).ToList()
                .ForEach(c => {
                    list.Add(new SelectListItem { Text = c.InspectClasses.ClassName,
                                                  Value = c.ClassID.ToString() });
                });
            return Json(list);
        }

        // POST: InspectDocDtlValSearch/GetItems
        [HttpPost]
        public JsonResult GetItems(int AreaID, string ClassID)
        {
            int classId = System.Convert.ToInt32(ClassID);
            List<SelectListItem> list = new List<SelectListItem>();
            db.InspectItems.Where(i => i.AreaID == AreaID && i.ClassID == classId)
                           .OrderBy(i => i.ItemOrder).ToList()
                .ForEach(i => {
                    list.Add(new SelectListItem
                    {
                        Text = i.ItemName,
                        Value = i.ItemID.ToString()
                    });
                });
            return Json(list);
        }
    }
}