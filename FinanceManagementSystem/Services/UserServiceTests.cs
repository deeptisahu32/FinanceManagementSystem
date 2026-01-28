using FinanceManagementSystem_Prj.Interfaces;
using FinanceManagementSystem_Prj.ViewModels;
using Moq;
using NUnit.Framework;
using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagementSystem.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<IUserRepository> _mockRepo;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepo.Object);
        }

        // first test case

        [Test]
        public void Login_ShouldReturn_NotFound_WhenUserDoesNotExist()
        {
            // ARRANGE
            var loginModel = new LoginViewModel
            {
                Username = "invalidUser",
                Password = "wrongPass"
            };

            _mockRepo.Setup(r =>
                r.GetUserByUsernameAndPassword(
                    loginModel.Username,
                    loginModel.Password
                )
            ).Returns((User)null);

            // ACT
            string result = _userService.Login(loginModel);

            // ASSERT
            Assert.That(result, Is.EqualTo("NOT_FOUND"));
        }

        // second test case
        [Test]
        public void Login_ShouldReturn_NotActive_WhenUserIsNotApproved()
        {
            // ARRANGE
            var loginModel = new LoginViewModel
            {
                Username = "ashok",
                Password = "1234"
            };

            var inactiveUser = new User
            {
                UserId = 1,
                Username = "ashok",
                IsActive = false   // Not approved by admin
            };

            _mockRepo.Setup(r =>
                r.GetUserByUsernameAndPassword(
                    loginModel.Username,
                    loginModel.Password
                )
            ).Returns(inactiveUser);

            // ACT
            string result = _userService.Login(loginModel);

            // ASSERT
            Assert.That(result, Is.EqualTo("NOT_ACTIVE"));
        }

        // third test case
        [Test]
        public void Login_ShouldReturn_Success_WhenUserIsActive()
        {
            // ARRANGE
            var loginModel = new LoginViewModel
            {
                Username = "sairam",
                Password = "1234"
            };

            var activeUser = new User
            {
                UserId = 2,
                Username = "sairam",
                IsActive = true   // Approved user
            };

            _mockRepo.Setup(r =>
                r.GetUserByUsernameAndPassword(
                    loginModel.Username,
                    loginModel.Password
                )
            ).Returns(activeUser);

            // ACT
            string result = _userService.Login(loginModel);

            // ASSERT
            Assert.That(result, Is.EqualTo("SUCCESS"));
        }
        //4th test case
        [Test]
        public void Register_ShouldThrowException_WhenDOBIsMissing()
        {
            // ARRANGE
            var registerModel = new RegisterViewModel
            {
                FullName = "Test User",
                Username = "testuser",
                Password = "1234",
                Email = "test@gmail.com",
                Phone = "9999999999",
                Address = "Hyderabad",

                DOB = null,

                BankName = "SBI",
                AccountNumber = "123456789",
                IFSCCode = "SBIN0001234"
            };

            // ACT + ASSERT
            var ex = Assert.Throws<Exception>(() =>
                _userService.Register(registerModel)
            );

            Assert.That(ex.Message, Is.EqualTo("Date of Birth is required"));
        }

        //5th test case
        [Test]
        public void GetUserForLogin_ShouldReturn_User_WhenCredentialsAreValid()
        {
            // ARRANGE
            var loginModel = new LoginViewModel
            {
                Username = "validuser",
                Password = "1234"
            };

            var expectedUser = new User
            {
                UserId = 10,
                Username = "validuser",
                IsActive = true
            };

            _mockRepo.Setup(r =>
                r.GetUserByUsernameAndPassword(
                    loginModel.Username,
                    loginModel.Password
                )
            ).Returns(expectedUser);

            // ACT
            var result = _userService.GetUserForLogin(loginModel);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("validuser"));
        }

    }
}
