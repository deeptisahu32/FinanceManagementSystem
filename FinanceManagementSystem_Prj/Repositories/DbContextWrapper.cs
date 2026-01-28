using FinanceManagementSystem_Prj.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManagementSystem_Prj.Models;
namespace FinanceManagementSystem_Prj.Repositories
{
    public class DbContextWrapper: IDbContext
    {
            //  Centralized
            //  Clean
            //  Easy to change later
        public Finance_Management_SystemEntities Context { get; }

        public DbContextWrapper()
        {
            Context = new Finance_Management_SystemEntities();
        }
    }
}