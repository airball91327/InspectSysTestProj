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
                string url = "WebApi/Accounts/CheckPasswd?id=" + model.UserName;
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
                    // Get user role
                    HttpClient clientRole = new HttpClient();
                    clientRole.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                    string urlRole = "WebApi/Accounts/GetRoles?id=" + model.UserName;
                    clientRole.DefaultRequestHeaders.Accept.Clear();
                    clientRole.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage responseRole = await clientRole.GetAsync(urlRole);
                    string rstrRole = "";
                    if (responseRole.IsSuccessStatusCode)
                    {
                        rstrRole = await responseRole.Content.ReadAsStringAsync();
                    }

                    // Get user real name
                    char[] charSpilt = new char[] { ',', '{', '}', '[', ']', '"', ':', '\\', '!', ';' };
                    string[] rstrSpilt = rstr.Split(charSpilt, StringSplitOptions.RemoveEmptyEntries);
                    string userRealName = rstrSpilt[1].ToString();

                    // Set user role and real name to userData.
                    string userData = rstrRole + userRealName;

                    // Set authentication cookie with userName, and userData(real name and user role)
                    //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var authTicket = new FormsAuthenticationTicket(
                    1,                             // version
                    model.UserName,                // user name
                    DateTime.Now,                  // created
                    DateTime.Now.AddMinutes(60),   // expires
                    model.RememberMe,              // persistent?
                    userData,                      // can be used to store roles
                    FormsAuthentication.FormsCookiePath
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    // Create the cookie.
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);                  
                    Response.Cookies.Add(authCookie);

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