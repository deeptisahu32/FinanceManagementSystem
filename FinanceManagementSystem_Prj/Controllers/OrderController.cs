using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class OrderController : Controller
    {
        Finance_Management_SystemEntities _context = new Finance_Management_SystemEntities();

        
        public ActionResult MyOrders()
        {
            try
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Account");

                int userId = Convert.ToInt32(Session["UserId"]);

                var orders = _context.Orders
                    .Include("Product")
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load your orders.";
                return RedirectToAction("Dashboard", "User");
            }
        }

    }
}