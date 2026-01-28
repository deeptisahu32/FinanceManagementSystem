using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class InfoController : Controller
    {
        // GET: Info
        public ActionResult Offers()
        {
            return View();
        }

        public ActionResult EmiZone()
        {
            return View();
        }

        //  Customer Support
        public ActionResult CustomerService()
        {
            return View();
        }

        //  Help & FAQ
        public ActionResult Help()
        {
            return View();
        }
    }
}