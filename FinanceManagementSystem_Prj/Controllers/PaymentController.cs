using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        Finance_Management_SystemEntities _context=new Finance_Management_SystemEntities();

        public ActionResult PayBeforeDueDate(int productId,int emiMonths)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            var product = _context.Products.Find(productId);
            if (product == null)
                return HttpNotFound();

            return View(product);
             
        }

        [HttpGet]
        public ActionResult BuyOnEMI(int productId)
        {
            //var product = _context.Products.Find(productId);
            //return View(product);
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            var product = _context.Products.Find(productId);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }
 

        [HttpPost]
        public ActionResult BuyOnEMI(int productId, int emiMonths)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Account");

                int userId = Convert.ToInt32(Session["UserId"]);

                var product = _context.Products.Find(productId);
                if (product == null)
                    return HttpNotFound();

                var emiCard = _context.EMICards
                    .FirstOrDefault(c => c.UserId == userId && c.IsActivated==true);

                if (emiCard == null)
                {
                    TempData["Error"] = "No active EMI card found.";
                    return RedirectToAction("Index", "Product");
                }

                //  STRICT LIMIT CHECK
                if (emiCard.AvailableLimit < product.Price)
                {
                    TempData["Error"] =
                        $"Insufficient EMI limit. Available: ₹{emiCard.AvailableLimit}";
                    return RedirectToAction("Index", "Product");
                }

                //  CREATE PURCHASE
                var purchase = new Purchase
                {
                    UserId = userId,
                    ProductId = productId,
                    CardId = emiCard.CardId,
                    EMIMonths = emiMonths,
                    TotalAmount = product.Price,
                    PurchaseDate = DateTime.Today
                };

                _context.Purchases.Add(purchase);
                _context.SaveChanges();


                //  CREATE EMI SCHEDULE
                decimal baseEmi = Math.Round(product.Price / emiMonths, 2);
                decimal allocated = 0;

                for (int i = 1; i <= emiMonths; i++)
                {
                    decimal emiAmount = (i == emiMonths)
                        ? product.Price - allocated
                        : baseEmi;

                    allocated += emiAmount;

                    _context.EMIPayments.Add(new EMIPayment
                    {
                        PurchaseId = purchase.PurchaseId,
                        EMISequence = i,
                        EMIAmount = emiAmount,
                        DueDate = purchase.PurchaseDate.Value.AddMonths(i),
                        PaymentStatus = "Pending",
                        PaymentMode = "EMI" //  IMPORTANT
                    });
                }
                var order = new Order
                {
                    UserId = userId,
                    ProductId = productId,
                    OrderAmount = product.Price,
                    PaymentMode = "EMI",
                    EmiDuration = emiMonths,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Completed"
                };

                _context.Orders.Add(order);

                //  UPDATE CARD LIMITS (ONCE PER PURCHASE)
                emiCard.UsedLimit += product.Price;
                emiCard.AvailableLimit = (decimal)(emiCard.CreditLimit - emiCard.UsedLimit);
                

                _context.SaveChanges();

                TempData["Success"] = "Purchase successful. EMI scheduled.";
                return RedirectToAction("MyEMIs", "User");
            }
            catch (Exception)
            {
                TempData["Error"] = "EMI purchase failed. Please try again.";
                return RedirectToAction("Index", "Product");
            }
        }


         

        [HttpGet]
        public ActionResult PayEMI(int emiPaymentId)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Account");

                var emi = _context.EMIPayments
                    .Include("Purchase")
                    .Include("Purchase.Product")
                    .Include("Purchase.EMICard")
                    .FirstOrDefault(e => e.PaymentId == emiPaymentId);

                if (emi == null)
                {
                    TempData["Error"] = "EMI not found.";
                    return RedirectToAction("MyEMIs", "User");
                }

                if (emi.PaymentStatus == "Paid")
                {
                    TempData["Error"] = "This EMI is already paid.";
                    return RedirectToAction("MyEMIs", "User");
                }

                DateTime today = DateTime.Today;
                DateTime purchaseDate = emi.Purchase.PurchaseDate ?? today;

                // =========================
                //  PAYMENT MODE RULES
                // =========================

                // EMI → only on due date
                if (emi.PaymentMode == "EMI" && today != emi.DueDate.Date)
                {
                    TempData["Error"] = "You can pay this EMI only on the due date.";
                    return RedirectToAction("MyEMIs", "User");
                }

                // DIRECT → between purchase date & due date
                if (emi.PaymentMode == "DIRECT" &&
                    (today < purchaseDate || today > emi.DueDate))
                {
                    TempData["Error"] = "Payment not allowed today for this EMI.";
                    return RedirectToAction("MyEMIs", "User");
                }

                //  Allowed → open payment page
                return View(emi);
            }
            catch
            {
                TempData["Error"] = "Unable to load payment page.";
                return RedirectToAction("MyEMIs", "User");
            }
        }
        [HttpPost]
        public ActionResult PayEMI(int emiPaymentId, string paymentMethod)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Account");

                var emi = _context.EMIPayments
                    .Include("Purchase")
                    .Include("Purchase.EMICard")
                    .FirstOrDefault(e => e.PaymentId == emiPaymentId);

                if (emi == null)
                    return HttpNotFound();

                if (emi.PaymentStatus == "Paid")
                {
                    TempData["Error"] = "EMI already paid.";
                    return RedirectToAction("MyEMIs", "User");
                }

                DateTime today = DateTime.Today;
                DateTime purchaseDate = emi.Purchase.PurchaseDate ?? today;

                // =========================
                //  RE-VALIDATE RULES
                // =========================

                if (emi.PaymentMode == "EMI" && today != emi.DueDate.Date)
                {
                    TempData["Error"] = "You can pay this EMI only on the due date.";
                    return RedirectToAction("MyEMIs", "User");
                }

                if (emi.PaymentMode == "DIRECT" &&
                    (today < purchaseDate || today > emi.DueDate))
                {
                    TempData["Error"] = "Payment not allowed today.";
                    return RedirectToAction("MyEMIs", "User");
                }

                // =========================
                //  PAYMENT SUCCESS
                // =========================

                emi.IsPaid = true;
                emi.PaymentStatus = "Paid";
                emi.PaymentDate = DateTime.Now;
                emi.PaymentMode = paymentMethod;

                // Restore card limit
                emi.Purchase.EMICard.AvailableLimit += emi.EMIAmount;
                emi.Purchase.EMICard.UsedLimit -= emi.EMIAmount;

                _context.SaveChanges();

                TempData["Success"] = "✅ EMI paid successfully.";
                return RedirectToAction("MyEMIs", "User");
            }
            catch
            {
                TempData["Error"] = "Something went wrong while paying EMI.";
                return RedirectToAction("MyEMIs", "User");
            }
        }

        //from here new code
        public ActionResult ChoosePaymentOption(int productId, string mode)
        {
            //var product = _context.Products.Find(productId);
            //if (product == null) return HttpNotFound();

            //return View(product);
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            ViewBag.Mode = mode;
            return View(product);
        }

        public ActionResult EMIPlan(int productId, string mode)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return HttpNotFound();

            ViewBag.Mode = mode;
            return View(product);
        }

        [HttpPost]
        public ActionResult CreateEMI(int productId, int emiMonths, string paymentMode)
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Account");

                int userId = Convert.ToInt32(Session["UserId"]);

                var product = _context.Products.Find(productId);
                if (product == null) return HttpNotFound();

                var card = _context.EMICards
                    .FirstOrDefault(c => c.UserId == userId && c.IsActivated == true);

                if (card == null)
                {
                    TempData["Error"] = "No active EMI card.";
                    return RedirectToAction("Index", "Product");
                }

                
                if (card.AvailableLimit < product.Price)
                {
                    TempData["InsufficientBalance"] =
                        $"❌ Insufficient limit. Available ₹{card.AvailableLimit}";

                    
                    // Stay on EMI page so popup can control redirect
                    return RedirectToAction(
                        "EMIPlan",
                        new { productId = productId, mode = paymentMode }
                    );
                }

                // ================= PURCHASE =================
                var purchase = new Purchase
                {
                    UserId = userId,
                    ProductId = productId,
                    CardId = card.CardId,
                    EMIMonths = emiMonths,
                    TotalAmount = product.Price,
                    PurchaseDate = DateTime.Today
                };

                _context.Purchases.Add(purchase);
                _context.SaveChanges();

                // ================= EMI SCHEDULE =================
                decimal baseEmi = Math.Round(product.Price / emiMonths, 2);
                decimal allocated = 0;

                for (int i = 1; i <= emiMonths; i++)
                {
                    decimal amount =
                        (i == emiMonths) ? product.Price - allocated : baseEmi;

                    allocated += amount;

                    _context.EMIPayments.Add(new EMIPayment
                    {
                        PurchaseId = purchase.PurchaseId,
                        EMISequence = i,
                        EMIAmount = amount,
                        DueDate = purchase.PurchaseDate.Value.AddMonths(i),
                        PaymentStatus = "Pending",
                        PaymentMode = paymentMode //  CORE DIFFERENCE
                    });
                }
                var order = new Order
                {
                    UserId = userId,
                    ProductId = productId,
                    OrderAmount = product.Price,
                    PaymentMode = paymentMode,
                    EmiDuration = emiMonths,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Completed"
                };

                _context.Orders.Add(order);

                // ================= CARD LIMIT UPDATE =================
                card.UsedLimit += product.Price;
                card.AvailableLimit = (decimal)(card.CreditLimit - card.UsedLimit);

                _context.SaveChanges();

                TempData["Success"] = "EMI created successfully.";
                return RedirectToAction("MyEMIs", "User");
            }
            catch
            {
                TempData["Error"] = "EMI creation failed.";
                return RedirectToAction("Index", "Product");
            }
        }

    }
}