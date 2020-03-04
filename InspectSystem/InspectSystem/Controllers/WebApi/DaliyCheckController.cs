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
    public class DaliyCheckController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: DaliyCheck
        public ActionResult Index()
        {
            return View();
        }

        // POST: /DaliyCheck/LoginCheck
        [HttpPost]
        public async Task<ApiResult> LoginCheck()
        {
            string debugString = "";
            DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
            DateTime dateTimeNowDate = DateTime.UtcNow.AddHours(8).Date;
            // If time is between 14:30 to 14:40
            if (dateTimeNow.Hour == 14 && dateTimeNow.Minute >=30 && dateTimeNow.Minute <= 40)
            {
                var areas = db.InspectAreas.ToList();
                var inspectDocs = db.InspectDocs.ToList();
                List<int> targetAreas = new List<int>();
                foreach(var item in areas)
                {
                    var targetAreaDoc = inspectDocs.Where(i => i.AreaID == item.AreaID)
                                                   .Where(i => i.Date.Date == dateTimeNowDate).FirstOrDefault();
                    // User isn't login.
                    if (targetAreaDoc == null && item.AreaID != 7)  //7為測試區域
                    {
                        targetAreas.Add(item.AreaID);
                    }
                }
                if (targetAreas.Count() > 0)
                {
                    string areaNames = "";
                    foreach(int areaId in targetAreas)
                    {
                        areaNames += "【" + db.InspectAreas.Find(areaId).AreaName + "】";
                    }
                    debugString += "Area :" + areaNames;
                    var areaCheckers = db.InspectAreaCheckers.Where(i => i.AreaID != 7).ToList();
                    //Send Mail
                    Mail mail = new Mail();
                    string body = "";
                    mail.from = "181316@cch.org.tw";
                    foreach (var checker in areaCheckers)
                    {
                        mail.to += checker.Email + ";";
                    }
                    mail.to = mail.to.TrimEnd(new char[] { ';' });
                    mail.subject = "巡檢系統[每日通知(人員尚未登入之區域)](測試信件)";
                    body += "<p>日期：" + dateTimeNow.Date.ToString("yyyy/MM/dd") + "</p>";
                    body += "<p>區域：" + areaNames + "</p>";
                    body += "<p>信件發送時間：" + DateTime.UtcNow.AddHours(8) + "</p>";
                    body += "<br/>";
                    body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    body += "<br/>";
                    mail.msg = body;

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                    string url = "WebApi/Mail/SendMail";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                }
                return new ApiResult(debugString);
                //return Json(debugString);
            }
            else
            {
                return new ApiResult("400", "執行失敗!");
                //return Json(debugString);
            }
        }

        // POST: /DaliyCheck/ProgressCheck
        [HttpPost]
        public async Task<ApiResult> ProgressCheck()
        {
            string debugString = "";
            DateTime dateTimeNow = DateTime.UtcNow.AddHours(8);
            DateTime dateTimeNowDate = DateTime.UtcNow.AddHours(8).Date;
            DateTime startDate = DateTime.UtcNow.AddHours(8).AddDays(-1);
            DateTime endDate = DateTime.UtcNow.AddHours(8).AddDays(1);
            // If time is between 17:00 to 17:10
            //if (dateTimeNow.Hour == 17 && dateTimeNow.Minute >= 00 && dateTimeNow.Minute <= 10)
            if (dateTimeNow.Hour >= 14)
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
                    //if (targetAreaDoc != null && item.AreaID != 7)
                    if (targetAreaDoc != null)
                    {
                        var isDocSend = inspectDocDetails.Where(i => i.AreaID == item.AreaID)
                                                         .Where(i => i.InspectDocs.Date.Date == dateTimeNowDate).FirstOrDefault();
                        // Doc isn't send to manager.
                        if (isDocSend == null)
                        {
                            targetAreas.Add(item.AreaID);
                        }
                    }
                }
                if (targetAreas.Count() > 0)
                {
                    foreach (int areaId in targetAreas)
                    {
                        var inspectDoc = inspectDocs.Where(i => i.AreaID == areaId)
                                                    .Where(i => i.Date.Date == dateTimeNowDate).FirstOrDefault();
                        var areaCheckers = db.InspectAreaCheckers.Where(i => i.AreaID != 7).ToList();
                        //Send Mail
                        Mail mail = new Mail();
                        string body = "";
                        mail.from = "181316@cch.org.tw";
                        //foreach (var checker in areaCheckers)
                        //{
                        //    mail.to += checker.Email + ";";
                        //}
                        //mail.to = mail.to.TrimEnd(new char[] { ';' });
                        //mail.to += inspectDoc.WorkerID + "@cch.org.tw";
                        mail.to = "airball91327@gmail.com";
                        mail.subject = "巡檢系統[每日通知(案件尚未送出之區域)](測試信件)";
                        body += "<p>日期：" + dateTimeNow.Date.ToString("yyyy/MM/dd") + "</p>";
                        body += "<p>區域：" + db.InspectAreas.Find(areaId).AreaName + "</p>";
                        body += "<p>巡檢人員：" + inspectDoc.WorkerName + "</p>";
                        body += "<p>信件發送時間：" + DateTime.UtcNow.AddHours(8) + "</p>";
                        body += "<br/>";
                        body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                        body += "<br/>";
                        mail.msg = body;

                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                        string url = "WebApi/Mail/SendMail";
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(url, content);
                    }
                }
                return new ApiResult(debugString);
                //return Json(debugString);
            }
            else
            {
                return new ApiResult("400", "執行失敗!");
                //return Json(debugString);
            }
        }
    }
}