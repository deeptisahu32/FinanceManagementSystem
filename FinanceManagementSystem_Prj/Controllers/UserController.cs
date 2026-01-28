using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class UserController : Controller
    {
        Finance_Management_SystemEntities _context = new Finance_Management_SystemEntities();


         
        public ActionResult Dashboard()
        {
            //  STEP 1: Session check (security)
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            //  STEP 2: Username for UI
            ViewBag.Username = Session["Username"];

            //  STEP 3: Get latest EMI card application of user
            var app = _context.UserCardApplications
    .FirstOrDefault(x => x.UserId == userId);

            if (app != null)
            {
                ViewBag.CardStatus = app.Status;
            }


            return View();
        }





        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        // GET: User/SelectCard

        public ActionResult SelectCard()
        {
            var cardTypes = _context.CardTypes
                                    .Where(c => c.IsActive == true)
                                    .ToList();

            return View(cardTypes);
        }


         

        // POST: User/ApplyCard
        public ActionResult ApplyEMICard()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            // Check if EMI card already exists
            var existingCard = _context.EMICards
                                       .FirstOrDefault(c => c.UserId == userId);

            if (existingCard != null)
            {
                // User already has card → show card details
                return RedirectToAction("MyCard");
            }

            // User does not have card → select card type
            return RedirectToAction("SelectCard");
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyCard(int cardTypeId)
        {
            try
            {
                //  STEP 1: Check login
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Account");

                int userId = Convert.ToInt32(Session["UserId"]);

                //  STEP 2: Prevent multiple applications
                var alreadyApplied = _context.UserCardApplications
                    .FirstOrDefault(x => x.UserId == userId);

                if (alreadyApplied != null)
                {
                    TempData["Error"] = "You have already applied for an EMI Card.";
                    return RedirectToAction("Dashboard");
                }

                //  STEP 3: Create new EMI card application
                var application = new UserCardApplication
                {
                    UserId = userId,
                    CardTypeId = cardTypeId,

                    Status = "PendingAdminApproval",

                    AppliedOn = DateTime.Now
                };

                _context.UserCardApplications.Add(application);
                _context.SaveChanges();

                TempData["Success"] =
                    "Your EMI Card application has been submitted and is pending admin approval.";

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                // ⚠ Exception handling
                TempData["Error"] = "Something went wrong. Please try again.";
                return RedirectToAction("SelectCard");
            }
        }



        public ActionResult PayJoiningFee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PayJoiningFeeConfirm()
        {
            //int userId = Convert.ToInt32(Session["UserId"]);
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);


            var app = _context.UserCardApplications
                .FirstOrDefault(x => x.UserId == userId);

            if (app == null || app.Status != "JoiningFeeRequested")
            {
                TempData["Error"] = "Invalid payment request";
                return RedirectToAction("Dashboard");
            }

            app.Status = "JoiningFeePaid";
            _context.SaveChanges();

            TempData["Success"] = "Joining fee paid successfully.";
            return RedirectToAction("Dashboard");
        }
        public ActionResult MyCard()
        {
            //  Security check
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            var card = _context.EMICards
                .Include("CardType")
                .FirstOrDefault(c =>
                    c.UserId == userId &&
                    c.IsActivated == true   //  FIX HERE
                );

            if (card == null)
            {
                TempData["Error"] = "No active EMI card found.";
                return RedirectToAction("Dashboard");
            }

            return View(card);
        }


        public ActionResult MyEMIs()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            var emis = _context.EMIPayments
                .Include("Purchase")
                .Include("Purchase.Product")
                .Include("Purchase.EMICard")
                .Where(e => e.Purchase.UserId == userId)
                 .OrderBy(e => e.DueDate).ToList();
            ViewBag.Today = DateTime.Today;
            return View(emis); // can be empty → handled in view
        }

        


    }
}