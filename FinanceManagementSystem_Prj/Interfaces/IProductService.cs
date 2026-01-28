using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using FinanceManagementSystem_Prj.Repositories;
using FinanceManagementSystem_Prj.Services;
namespace FinanceManagementSystem_Prj.Interfaces
{
    public interface IProductService
    {
        List<Product> GetProducts();

    }
}
