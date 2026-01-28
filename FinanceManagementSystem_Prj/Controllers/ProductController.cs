 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinanceManagementSystem_Prj.Repositories;
using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.Interfaces;
using FinanceManagementSystem_Prj.Services;

namespace FinanceManagementSystem_Prj.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController()
        {
            _productService = new ProductService();
        }


        Finance_Management_SystemEntities _context = new Finance_Management_SystemEntities();

        public ActionResult Index(string category, string search)
        {
            var products = _context.Products.Where(p => (bool)p.IsActive);

            //  Search
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search));
            }

            //  Category filter
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            // Sidebar categories
            ViewBag.Categories = _context.Products
                .Where(p => (bool)p.IsActive)
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            return View(products.ToList());
        }
        public ActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.IsEMIAvailable = product.Price >= 5000;
            return View(product);
        }
        public ActionResult Buy(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }
    }
}