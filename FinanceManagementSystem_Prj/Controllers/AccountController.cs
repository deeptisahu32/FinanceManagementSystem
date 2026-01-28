using FinanceManagementSystem_Prj.Interfaces;
using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.Services;
using FinanceManagementSystem_Prj.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;



namespace FinanceManagementSystem_Prj.Controllers
{
    public class AccountController : Controller
    {
        Finance_Management_SystemEntities _context= new Finance_Management_SystemEntities();
        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44362/");

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = client.PostAsync("api/users/register", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("RegisterSuccess");
                }

                // API error visible to user
                ModelState.AddModelError("", "Registration failed. Please try again.");
            }

            return View(model);
        }
 

         

        // GET: Account/RegisterSuccess
        public ActionResult RegisterSuccess()
        {
            return View();
        }


        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
         
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44362/");

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = client.PostAsync("api/auth/login", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    //  Read API response
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    // Example JSON → deserialize
                    dynamic result = JsonConvert.DeserializeObject(responseData);

                    //  SET SESSION
                    Session["UserId"] = result.userId;
                    Session["Username"] = result.username;

                    //  Redirect to USER dashboard
                    return RedirectToAction("Dashboard", "User");
                }

                ModelState.AddModelError("", response.Content.ReadAsStringAsync().Result);
            }

            return View(model);
        }

        //Get forget password
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email.");
                return View(model);
            }

            return RedirectToAction("ResetPassword", new { id = user.UserId });
        }
        // For Reset Password
        public ActionResult ResetPassword(int id)
        {
            return View(new ResetPasswordViewModel { UserId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.Find(model.UserId);
            if (user == null)
                return RedirectToAction("Login");

            user.PasswordHash = model.NewPassword; // hash later
            _context.SaveChanges();

            TempData["Success"] = "Password reset successfully.";
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Logout()
        {
            // Clear authentication cookie
            FormsAuthentication.SignOut();

            // Clear session
            Session.Clear();
            Session.Abandon();

            // Prevent back button access
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            // Redirect to login page
            return RedirectToAction("Login", "Account");
        }

    }
}
