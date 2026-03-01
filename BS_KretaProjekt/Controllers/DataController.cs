using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles ="Tanar")]
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
        [Authorize(Roles = "Tanar")]
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
        [Authorize(Roles = "Tanar")]
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
        [Authorize(Roles = "Tanar")]
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

        [Authorize(Roles = "Tanar")]
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
        [Authorize(Roles = "Tanar")]
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
