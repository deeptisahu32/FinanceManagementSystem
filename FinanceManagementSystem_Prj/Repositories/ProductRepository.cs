using FinanceManagementSystem_Prj.Interfaces;
using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanceManagementSystem_Prj.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly Finance_Management_SystemEntities _context;

        // PARAMETERLESS CONSTRUCTOR (ADD THIS)
        public ProductRepository()
        {
            _context = new Finance_Management_SystemEntities();
        }

        public List<Product> GetAllActiveProducts()
        {
            return _context.Products
                           .Where(p => (bool)p.IsActive)
                           .ToList();
        }
    }
    //Repository talks to DB only, nothing else.
}