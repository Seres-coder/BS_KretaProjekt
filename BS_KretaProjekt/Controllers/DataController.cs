using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
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

        [HttpGet("diaklistazasa")]
        public ActionResult<IEnumerable<StudentDto>> GetDiak()
        {
            try
            {
                return Ok(_model.GetDiak());
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        [HttpGet("tanarlistazasa")]
        public ActionResult<IEnumerable<StudentDto>> GetTeacher()
        {
            try
            {
                return Ok(_model.GetTeacher());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("modifystudentdata")]
        public async Task<ActionResult> ModifyStudetData([FromBody] StudentDto dto)
        {
            try
            {
                await _model.ModifyStudetData(dto);
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
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("addstudentdata")]
        public async Task<ActionResult> AddStudentData([FromBody] StudentDto dto)
        {
            try
            {
                await _model.AddStudentData(dto);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("addteacherdata")]
        public async Task<ActionResult> AddTeacherData([FromBody] TeacherDto dto)
        {
            try
            {
                await _model.AddTeacherData(dto);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
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
