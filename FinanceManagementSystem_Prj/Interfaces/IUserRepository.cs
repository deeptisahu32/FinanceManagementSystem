using FinanceManagementSystem_Prj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagementSystem_Prj.Interfaces
{
    public interface IUserRepository
    {
        void RegisterUser(User user, UserBankDetail bankDetail);
        User GetUserByUsernameAndPassword(string username, string password);

    }
}
