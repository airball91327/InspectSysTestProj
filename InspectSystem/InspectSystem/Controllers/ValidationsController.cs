using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;
using InspectSystem.Controllers;

namespace InspectSystem.Controllers
{
    public class ValidationsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Validations/FieldID
        public ActionResult FieldID(InspectFields inspectFields)
        {
            var ACID = TempData["CreateACID"];
            var itemID = TempData["CreateItemID"];
            TempData.Keep();
            var fieldID = inspectFields.FieldID; 

            string message = null;
            var FindFieldID = db.InspectFields.Find(ACID, itemID, fieldID);

            if( FindFieldID != null )
            {
                message = "編號重複";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "可用編號";
                return Json(message, JsonRequestBehavior.AllowGet);
            }

        }
    }
}