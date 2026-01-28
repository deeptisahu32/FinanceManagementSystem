using FinanceManagementSystem_Prj.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManagementSystem_Prj.Models;

namespace FinanceManagementSystem_Prj.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly Finance_Management_SystemEntities _context;

        public UserRepository()
        {
            _context = new Finance_Management_SystemEntities();
        }

        public void RegisterUser(User user, UserBankDetail bankDetail)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            bankDetail.UserId = user.UserId;
            _context.UserBankDetails.Add(bankDetail);
            _context.SaveChanges();
        }

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return _context.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.PasswordHash == password
            );
        }


    }
}