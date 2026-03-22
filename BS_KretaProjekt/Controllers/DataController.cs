using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BS_KretaProjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private DataModel _model;
        public DataController(DataModel model)
        {
            _model = model;
        }

        [HttpGet("mydata")]
        public async Task<ActionResult<StudentDto>> GetMyData([FromQuery] int userId)
        {
            try
            {
                var response = await _model.GetMyData(userId);

                if (response == null)
                {
                    return NotFound("Nincs ilyen diákadat.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Szerverhiba: {ex.Message}");
            }
        }

        [HttpGet("myteacherdata")]
        public async Task<ActionResult<StudentDto>> GetMyTeacherData([FromQuery] int userId)
        {
            try
            {
                var response = await _model.GetMyTeacherData(userId);

                if (response == null)
                {
                    return NotFound("Nincs ilyen tanáradat.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Szerverhiba: {ex.Message}");
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("diaklistazasa")]
        public async Task <ActionResult<IEnumerable<StudentDto>>> GetDiak()
        {
            try
            {
                var response= await _model.GetDiak();
                return Ok(response);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpGet("tanarlistazasa")]
        public async Task <ActionResult<IEnumerable<StudentDto>>> GetTeacher()
        {
            try
            {
                await _model.GetTeacher();
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("modifystudentdata")]
        public async Task<ActionResult> ModifyStudetData([FromBody] StudentDto dto)
        {
            try
            {
                await _model.ModifyStudentData(dto);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("modifyteacherdata")]
        public async Task<ActionResult> ModifyTeacherData([FromBody] TeacherDto dto)
        {
            try
            {
                await _model.ModifyTeacherData( dto);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deletestudentdata")]
        public   async Task<ActionResult> DeleteStudentData([FromQuery] int id)
        {
            try
            {
                await _model.DeleteStudentData(id);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteteacherdata")]
        public async Task<ActionResult> DeleteTeacherData([FromQuery] int id)
        {
            try
            {
                await _model.DeleteTeacherData(id);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }





    }
}
