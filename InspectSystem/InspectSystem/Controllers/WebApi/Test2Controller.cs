using InspectSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers.WebApi
{
    public class Test2Controller : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Test2
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Test2/DaliyLoginCheck
        [HttpPost]
        public async Task<ApiResult> DaliyLoginCheck()
        {
            string debugString = "";
            DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
            DateTime dateTimeNowDate = DateTime.UtcNow.AddHours(8).Date;
            // If time is between 14:30 to 14:50
            if (dateTimeNow.Hour >= 14 && dateTimeNow.Minute >=30 && dateTimeNow.Minute <= 50)
            {
                var areas = db.InspectAreas.ToList();
                var inspectDocs = db.InspectDocs.ToList();
                List<int> targetAreas = new List<int>();
                foreach(var item in areas)
                {
                    var targetAreaDoc = inspectDocs.Where(i => i.AreaID == item.AreaID)
                                                   .Where(i => i.Date.Date == dateTimeNowDate).FirstOrDefault();
                    // User isn't login.
                    if (targetAreaDoc == null)
                    {
                        targetAreas.Add(item.AreaID);
                        debugString += "Area :" + item.AreaID;
                    }
                }
                if (targetAreas.Count() > 0)
                {
                    foreach(int areaId in targetAreas)
                    {
                        var areaCheckers = db.InspectAreaCheckers.Where(i => i.AreaID == areaId);
                        //Send Mail
                        Mail mail = new Mail();
                        string body = "";
                        //mail.from = inspectDocFlow.CheckerID + "@cch.org.tw";
                        foreach (var checker in areaCheckers)
                        {
                            mail.to += checker.Email + ";";
                            debugString += "checkerID:" + checker.CheckerID + "Email:" + checker.Email;
                        }
                        mail.subject = "巡檢系統[每日尚未登入通知]";
                        body += "<p>日期：" + dateTimeNow.Date.ToString("yyyy/MM/dd") + "</p>";
                        body += "<p>區域：" + db.InspectAreas.Find(areaId).AreaName + "</p>";
                        body += "<br/>";
                        body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                        body += "<br/>";
                        mail.msg = body;

                        //HttpClient client = new HttpClient();
                        //client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                        //string url = "WebApi/Mail/SendMail";
                        //client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Accept.Add(
                        //    new MediaTypeWithQualityHeaderValue("application/json"));
                        //var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
                        //HttpResponseMessage response = await client.PostAsync(url, content);
                    }
                }
                return new ApiResult(debugString);
            }
            else
            {
                return new ApiResult("400", "執行失敗!");
            }
        }

        // POST: /Test2/DaliyProgressCheck
        [HttpPost]
        public async Task<ActionResult> DaliyProgressCheck()
        {
            string debugString = "";
            DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
            DateTime dateTimeNowDate = DateTime.UtcNow.AddHours(8).Date;
            DateTime startDate = DateTime.UtcNow.AddHours(8).AddDays(-1);
            DateTime endDate = DateTime.UtcNow.AddHours(8).AddDays(1);
            // If time is between 17:00 to 17:20
            if (dateTimeNow.Hour >= 17 && dateTimeNow.Minute >= 00 && dateTimeNow.Minute <= 50)
            {
                var areas = db.InspectAreas.ToList();
                var inspectDocs = db.InspectDocs.ToList();
                var inspectDocDetails = db.InspectDocDetails.Where(i => i.InspectDocs.Date >= startDate)
                                                            .Where(i => i.InspectDocs.Date <= endDate)
                                                            .Include(i => i.InspectDocs).ToList();
                List<int> targetAreas = new List<int>();
                foreach (var item in areas)
                {
                    var targetAreaDoc = inspectDocs.Where(i => i.AreaID == item.AreaID)
                                                   .Where(i => i.Date.Date == dateTimeNowDate).FirstOrDefault();
                    if (targetAreaDoc != null)
                    {
                        var isDocSend = inspectDocDetails.Where(i => i.AreaID == item.AreaID)
                                                         .Where(i => i.InspectDocs.Date.Date == dateTimeNowDate).FirstOrDefault();
                        // Doc isn't send to manager.
                        if (isDocSend == null)
                        {
                            targetAreas.Add(item.AreaID);
                            debugString += "Area :" + item.AreaID;
                        }
                    }
                }
                if (targetAreas.Count() > 0)
                {
                    foreach (int areaId in targetAreas)
                    {
                        var areaCheckers = db.InspectAreaCheckers.Where(i => i.AreaID == areaId);
                        //Send Mail
                        Mail mail = new Mail();
                        string body = "";
                        //mail.from = inspectDocFlow.CheckerID + "@cch.org.tw";
                        foreach (var checker in areaCheckers)
                        {
                            mail.to += checker.Email + ";";
                            debugString += "checkerID:" + checker.CheckerID + "Email:" + checker.Email;
                        }
                        mail.subject = "巡檢系統[每日巡檢尚未完成通知]";
                        body += "<p>日期：" + dateTimeNow.Date.ToString("yyyy/MM/dd") + "</p>";
                        body += "<p>區域：" + db.InspectAreas.Find(areaId).AreaName + "</p>";
                        body += "<br/>";
                        body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                        body += "<br/>";
                        mail.msg = body;

                        //HttpClient client = new HttpClient();
                        //client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                        //string url = "WebApi/Mail/SendMail";
                        //client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Accept.Add(
                        //    new MediaTypeWithQualityHeaderValue("application/json"));
                        //var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
                        //HttpResponseMessage response = await client.PostAsync(url, content);
                    }
                }
                //return new ApiResult(debugString);
                return Json(debugString);
            }
            else
            {
                //return new ApiResult("400", "執行失敗!");
                return Json(debugString);
            }
        }
    }
}