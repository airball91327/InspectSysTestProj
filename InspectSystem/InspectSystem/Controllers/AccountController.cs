using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                string url = "WebApi/api/Accounts?id=" + model.UserName;
                url += "&pwd=" + HttpUtility.UrlEncode(model.Password, Encoding.GetEncoding("UTF-8"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(url);
                string rstr = "";
                if (response.IsSuccessStatusCode)
                {
                    rstr = await response.Content.ReadAsStringAsync();
                }

                /* If the UserName and the Password are legal. */
                if (rstr.Contains("成功"))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);                  
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "登入無效.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }   
    }
}