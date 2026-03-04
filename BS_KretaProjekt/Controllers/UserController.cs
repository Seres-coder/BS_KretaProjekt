using BS_KretaProjekt.Dto;
using System.Security.Claims;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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
        [HttpPut("updatepassword")]
        public async Task<IActionResult> UpdatePassword([FromQuery] int userid, [FromQuery] string password)
        {
            try
            {
                await _model.ChangePassword(userid, password);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Registration
        [HttpPost("registration")]
        public async Task<IActionResult> RegistrationController([FromQuery] string name, [FromQuery] string password)
        {
            try
            {
                await _model.Registration(name, password);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // "mar letezik"
            {
                return Conflict(ex.Message);
            }
        }
        #endregion

        #region Login checker
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginController([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                var user = _model.ValidateUser(username, password);

                if (user == null)
                    return Unauthorized();

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.NameIdentifier, user._user_id.ToString()),
                    new Claim(ClaimTypes.Name, user._belepesnev),
                    new Claim(ClaimTypes.Role, user._Role)
                };

                var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(id);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Role update
        [Authorize(Roles = "Tanar")]
        [HttpPut("upgraderole")]
        public async Task<IActionResult> UpdateRole([FromQuery] int id, [FromQuery] string tantargy)
        {
            try
            {
                await _model.PromoteToTanar(id, tantargy);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}