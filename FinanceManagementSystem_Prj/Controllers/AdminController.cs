using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class AdminController : Controller
    {
        private Finance_Management_SystemEntities _context;

        public AdminController()
        {
            _context = new Finance_Management_SystemEntities();
        }



        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var admin = _context.Admins.FirstOrDefault(a =>
                    a.Username == model.Username &&
                    a.PasswordHash == model.Password
                );

                if (admin != null)
                {
                    //  SET ADMIN SESSION
                    Session["AdminId"] = admin.AdminId;
                    Session["AdminUsername"] = admin.Username;

                    return RedirectToAction("Dashboard");
                }

                ModelState.AddModelError("", "Invalid admin credentials");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Login failed: " + ex.Message);
                return View(model);
            }
        }


        public ActionResult Dashboard()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Login");
            }
            try
            {

                // Just dashboard landing page
                ViewBag.TotalUsers = _context.Users.Count();
                ViewBag.PendingUsers = _context.Users.Count(u => u.IsActive == false);
                ViewBag.PendingCards = _context.UserCardApplications.Count(a => a.Status == "PendingAdminApproval");
                ViewBag.ActiveCards = _context.EMICards.Count();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

       
        public ActionResult PendingUsers()
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            var pendingUsers = (
                from u in _context.Users
                join b in _context.UserBankDetails
                    on u.UserId equals b.UserId
                where u.IsActive == false
                select new PendingUserViewModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Username = u.Username,
                    BankName = b.BankName,
                    AccountNumber = b.AccountNumber,
                    IFSCCode = b.IFSCCode
                }
            ).ToList();

            return View(pendingUsers);
        }
       
        public ActionResult Approve(int id)
        {
            try
            {
                if (Session["AdminId"] == null)
                    return RedirectToAction("Login");

                var user = _context.Users.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                    return HttpNotFound();

                user.IsActive = true;
                _context.SaveChanges();

                // CLEAR REJECT MESSAGE
                TempData.Remove("Error");

                // SET APPROVE MESSAGE
                TempData["Success"] = "User approved successfully.";

                return RedirectToAction("Dashboard");
            }
            catch
            {
                TempData["Error"] = "Something went wrong while approving user.";
                return RedirectToAction("Dashboard");
            }
        }


        
        public ActionResult Reject(int id)
        {
            try
            {
                if (Session["AdminId"] == null)
                    return RedirectToAction("Login");

                var user = _context.Users.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                    return HttpNotFound();

                user.IsActive = false;
                _context.SaveChanges();

                //  CLEAR APPROVE MESSAGE
                TempData.Remove("Success");

                //  SET REJECT MESSAGE
                TempData["Error"] = "User rejected successfully.";

                return RedirectToAction("Dashboard");
            }
            catch
            {
                TempData["Error"] = "Something went wrong while rejecting user.";
                return RedirectToAction("Dashboard");
            }
        }

        public ActionResult UserDetails(int id)
        {
            //  Admin guard
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            var user = _context.Users
                .Include("UserBankDetails")
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }
        public ActionResult PendingApplications()
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login", "Admin");

            var apps = _context.UserCardApplications
                .Where(x =>
                    x.Status == "PendingAdminApproval" ||
                    x.Status == "JoiningFeeRequested" ||
                    x.Status == "JoiningFeePaid")
                .ToList();

            return View(apps);
        }

 

        public ActionResult RequestJoiningFee(int id)
        {
            var app = _context.UserCardApplications
                .FirstOrDefault(x => x.ApplicationId == id);

            if (app == null)
            {
                TempData["Error"] = "Application not found";
                return RedirectToAction("PendingApplications");
            }

            app.Status = "JoiningFeeRequested";
            _context.SaveChanges();

            TempData["Success"] = "Joining fee request sent to user.";
            return RedirectToAction("PendingApplications");
        }
         
        public ActionResult ApproveEMICard(int id)
        {
            try
            {
                //  Load application WITH CardType
                var app = _context.UserCardApplications
                                  .Include(a => a.CardType)
                                  .FirstOrDefault(a => a.ApplicationId == id);

                if (app == null)
                {
                    TempData["Error"] = "Application not found.";
                    return RedirectToAction("PendingApplications");
                }

                //  Status validation
                if (app.Status != "JoiningFeePaid")
                {
                    TempData["Error"] = "Joining fee not paid yet.";
                    return RedirectToAction("PendingApplications");
                }

                //  Prevent duplicate EMI card
                var existingCard = _context.EMICards
                                           .FirstOrDefault(c => c.UserId == app.UserId);

                if (existingCard != null)
                {
                    TempData["Error"] = "EMI Card already issued for this user.";
                    return RedirectToAction("PendingApplications");
                }

                //   credit limit read
                decimal limit = app.CardType != null ? app.CardType.CardLimit : 0;

                //  Create EMI Card
                EMICard card = new EMICard
                {
                    UserId = app.UserId,
                    CardTypeId = app.CardTypeId,
                    CardNumber = "EMI" + DateTime.Now.Ticks,
                    CreditLimit = limit,
                    AvailableLimit = limit,
                    UsedLimit = 0,
                    ValidTill = DateTime.Today.AddYears(3),
                    IsActivated = true,
                    CreatedDate = DateTime.Now
                };

                _context.EMICards.Add(card);

                //  Update application status
                app.Status = "CardIssued";

                _context.SaveChanges();

                TempData["Success"] = "EMI Card issued successfully.";
                return RedirectToAction("PendingApplications");
            }
            catch (Exception ex)
            {
                 
                TempData["Error"] = "Failed to issue EMI card: " + ex.Message;
                return RedirectToAction("PendingApplications");
            }
        }



        public ActionResult FeePaidApplications()
        {
            var apps = _context.UserCardApplications
                .Where(x => x.Status == "FeePaid")
                .ToList();

            return View(apps);
        }
        public ActionResult IssueEMICard(int id)
        {
            var app = _context.UserCardApplications.Find(id);

            if (app == null || app.Status != "FeePaid")
            {
                TempData["Error"] = "Payment not completed.";
                return RedirectToAction("FeePaidApplications");
            }

            EMICard card = new EMICard
            {
                UserId = app.UserId,
                CardTypeId = app.CardTypeId,
                CardNumber = "EMI" + DateTime.Now.Ticks,
                UsedLimit = 0,
                ValidTill = DateTime.Now.AddYears(3),
                IsActivated = true
            };

            app.Status = "CardIssued";

            _context.EMICards.Add(card);
            _context.SaveChanges();

            TempData["Success"] = "EMI Card issued successfully.";
            return RedirectToAction("FeePaidApplications");
        }
        public ActionResult Products()
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            try
            {
                var products = _context.Products.ToList();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        //Add product (GET)
        public ActionResult AddProduct()
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            return View();
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(Product product, HttpPostedFileBase ProductImage)
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            try
            {
                if (ProductImage != null && ProductImage.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(ProductImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/ProductImages"), fileName);
                    ProductImage.SaveAs(path);
                    product.ImageUrl = fileName;
                }

                if (!ModelState.IsValid)
                    return View(product);

                product.IsActive = true;
                _context.Products.Add(product);
                _context.SaveChanges();

                TempData["Success"] = "Product added successfully";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }


        // EDIT PRODUCT - GET
        // =========================
        public ActionResult EditProduct(int id)
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                    return HttpNotFound();

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Products");
            }
        }

         
        // =========================
        // EDIT PRODUCT - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product, HttpPostedFileBase ProductImage)
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            try
            {
                var existing = _context.Products.Find(product.ProductId);
                if (existing == null)
                    return HttpNotFound();

                // ✅ Update basic fields
                existing.ProductName = product.ProductName;
                existing.Description = product.Description;
                existing.Price = product.Price;

                // ✅ If admin uploaded new image
                if (ProductImage != null && ProductImage.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(ProductImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/ProductImages"), fileName);

                    ProductImage.SaveAs(path);

                    existing.ImageUrl = fileName; // save new image
                }
                // else → keep old image

                _context.SaveChanges();

                TempData["Success"] = "Product updated successfully";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }


        // =========================
        // DELETE PRODUCT
        // =========================
        public ActionResult DeleteProduct(int id)
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                    return HttpNotFound();

                _context.Products.Remove(product);
                _context.SaveChanges();

                TempData["Success"] = "Product deleted successfully";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Products");
            }
        }

        // =========================
        // VIEW USER ORDERS
        // =========================
        public ActionResult Orders()
        {
            if (Session["AdminId"] == null)
                return RedirectToAction("Login");

            try
            {
                var orders = _context.Purchases
                    .Include(p => p.User)
                    .Include(p => p.Product)
                    .OrderByDescending(p => p.PurchaseDate)
                    .ToList();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }








        //for logout 
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Admin");
        }




    }
}