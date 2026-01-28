using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagementSystem_Prj.Interfaces
{
    public interface IDbContext
    {
        Finance_Management_SystemEntities Context { get; }

    }
}
