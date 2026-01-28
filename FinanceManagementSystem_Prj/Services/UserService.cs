using FinanceManagementSystem_Prj.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.Repositories;
using FinanceManagementSystem_Prj.ViewModels;

namespace FinanceManagementSystem_Prj.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void Register(RegisterViewModel model)
        {
            //  SAFETY CHECK (VERY IMPORTANT)
            if (!model.DOB.HasValue)
                throw new Exception("Date of Birth is required");

            var user = new User
            {
                FullName = model.FullName,
                DOB = model.DOB.Value,   //  SAFE NOW
                Email = model.Email,
                Phone = model.Phone,
                Username = model.Username,
                PasswordHash = model.Password, // hashing later
                Address = model.Address,
                IsActive = false,
                 CreatedDate = DateTime.Now

            };

            var bank = new UserBankDetail
            {
                BankName = model.BankName,
                AccountNumber = model.AccountNumber,
                IFSCCode = model.IFSCCode
            };

            _userRepository.RegisterUser(user, bank);
        }

        public string Login(LoginViewModel model)
        {
            var user = _userRepository.GetUserByUsernameAndPassword(
                model.Username,
                model.Password
            );

            if (user == null)
                return "NOT_FOUND";

            if ((bool)!user.IsActive)
                return "NOT_ACTIVE";

            return "SUCCESS";
        }
        public User GetUserForLogin(LoginViewModel model)
        {
            return _userRepository.GetUserByUsernameAndPassword(
                model.Username,
                model.Password
            );
        }

        

    }
}