using BS_KretaProjekt.Dto;
using System.Security.Claims;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BS_KretaProjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserModel _model;
        public UserController(UserModel model)
        {
            _model = model;
        }

        #region Password Change
        [Authorize]
        [HttpPut("updatepassword")]
        public ActionResult UpdatePassword(int userid, string password)
        {
            try
            {
                _model.ChangePassword(userid, password);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        #endregion

        #region Registration(At first you can only registrate as a student,the admin can upgrade to being teacher)

        [HttpPost("registration")]
        public ActionResult RegistrationController(string name, string password)
        {
            try
            {
                _model.Registration(name, password);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Login checker

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginController(string username, string password)
        {
            try
            {
                var user = _model.ValidateUser(username, password);
                if (null == user)
                {
                    return Unauthorized();
                }
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.NameIdentifier,user._user_id.ToString()),
                    new Claim(ClaimTypes.Name,user._belepesnev),
                    new Claim(ClaimTypes.Role,user._Role)
                };
                var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(id);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Role update
        [Authorize(Roles = "Admin")]
        [HttpPut("upgraderole")]
        public ActionResult UpdateRole(int id, string tantargy)
        {
            try
            {
                _model.PromoteToTanar(id, tantargy);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        #endregion
    }
}
