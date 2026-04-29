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
        //PUT /api/user/updatepassword – megváltoztatja a felhasználó jelszavát
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
                { return StatusCode(406, ex.Message); }
            }
            catch (InvalidOperationException ex)
            {
                { return BadRequest(ex.Message); }
            }
        }
        #endregion

        #region Registration
        // POST /api/user/registration – regisztrál egy új felhasználót diák szerepkörrel
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
                { return StatusCode(406, ex.Message); }
            }
            catch (InvalidOperationException ex) 
            {
                return Conflict(ex.Message);
            }
        }
        #endregion

        #region Login checker
        // POST /api/user/login – ellenőrzi a belépési adatokat, sikeres login esetén cookie-t állít be
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

                return Ok(new
                {
                    id = user._user_id,
                    name = user._belepesnev,
                    role = user._Role
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Role update
        //PUT /api/user/upgraderole – diákból tanárrá lépteti elő a felhasználót
        [Authorize(Roles = "Admin")]
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
                { return StatusCode(406, ex.Message); }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        [HttpPost("registerdiak")]
        public async Task<ActionResult> RegisterDiak([FromBody] RegisterStudentDto dto)
        {
            try
            {
                await _model.RegisterDiakByAdmin(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("registertanar")]
        public async Task<ActionResult> RegisterTanar([FromBody] RegisterTeacherDto dto)
        {
            try
            {
                await _model.RegisterTanarByAdmin(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


    }
}