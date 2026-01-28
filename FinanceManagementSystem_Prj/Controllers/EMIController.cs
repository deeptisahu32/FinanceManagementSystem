using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class EMIController : Controller
    {
        // GET: EMI
        Finance_Management_SystemEntities _context=new Finance_Management_SystemEntities();
        public ActionResult MyEMIs()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);


            var emis = _context.EMIPayments
                .Where(e => e.Purchase.UserId == userId)
                .OrderBy(e => e.PaymentDate)
                .ToList();

            return View(emis);
        }
        public ActionResult PayEMI(int id)
        {
            var emi = _context.EMIPayments.Find(id);

            if (emi.PaymentDate > DateTime.Now)
            {
                TempData["Error"] = "EMI not due yet";
                return RedirectToAction("MyEMIs");
            }

            emi.PaymentStatus = "Paid";

            var card = _context.EMICards
                .First(c => c.CardId == emi.Purchase.CardId);

            card.AvailableLimit += emi.EMIAmount;
            card.UsedLimit -= emi.EMIAmount;

            _context.SaveChanges();

            return RedirectToAction("MyEMIs");
        }


    }
}