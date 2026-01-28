using System.Web.Http;
using FinanceManagementSystem_Prj.Services;
using FinanceManagementSystem_Prj.ViewModels;

namespace FinanceManagementSystem_Prj.Controllers.API
{
    [RoutePrefix("api/auth")]
    public class AuthApiController : ApiController
    {
        private readonly UserService _userService;

        public AuthApiController()
        {
            _userService = new UserService();
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginViewModel model)
        {
            var user = _userService.GetUserForLogin(model);

            if (user == null)
                return BadRequest("INVALID");

            if ((bool)!user.IsActive)
                return BadRequest("NOT_ACTIVE");

            return Ok(new
            {
                userId = user.UserId,
                username = user.Username
            });
        }


    }
}

