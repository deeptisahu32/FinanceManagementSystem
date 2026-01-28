using FinanceManagementSystem_Prj.Interfaces;
using FinanceManagementSystem_Prj.Models;
using FinanceManagementSystem_Prj.Services;
using FinanceManagementSystem_Prj.ViewModels;
using System;
using System.Web.Http;

namespace FinanceManagementSystem_Prj.Controllers.API
{
    [RoutePrefix("api/users")]
    public class UsersApiController : ApiController
    {
        private readonly IUserService _userService;

         public UsersApiController()
        {
            _userService = new UserService();
        }

        // POST: api/users/register
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
  
            _userService.Register(model);
            return Ok("User registered successfully");
        }
    }
}
