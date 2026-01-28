using FinanceManagementSystem_Prj.Interfaces;
using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManagementSystem_Prj.Repositories;
 
namespace FinanceManagementSystem_Prj.Services
{
     
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService()
        {
            _productRepository = new ProductRepository();
        }

        public List<Product> GetProducts()
        {
            // Future business rules go here
            return _productRepository.GetAllActiveProducts();
        }
    }
}