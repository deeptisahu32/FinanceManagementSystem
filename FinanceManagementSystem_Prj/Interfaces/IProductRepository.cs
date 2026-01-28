using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManagementSystem_Prj.Services;
using FinanceManagementSystem_Prj.Repositories;
namespace FinanceManagementSystem_Prj.Interfaces
{
    public  interface IProductRepository
    {
        List<Product> GetAllActiveProducts();

    }
}
