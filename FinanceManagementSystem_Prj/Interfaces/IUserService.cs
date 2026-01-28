using FinanceManagementSystem_Prj.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagementSystem_Prj.Interfaces
{
    public interface IUserService
    {
        void Register(RegisterViewModel model);

    }
}
