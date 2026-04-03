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
        [HttpGet("getmydata")]
        public async Task<ActionResult<StudentDto>> GetMyData(int user_id)
        {
            try
            {
                var response = await _model.GetMyData(user_id);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    
        [HttpGet("getmyteacherdata")]
        public async Task<ActionResult<StudentDto>> GetMyTeacherData(int user_id)
        {
            try
            {
                var response = await _model.GetMyTeacherData(user_id);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        
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
 
        [HttpGet("tanarlistazasa")]
        public async Task <ActionResult<IEnumerable<TeacherDto>>> GetTeacher()
        {
            try
            {
                var response = await _model.GetTeacher();
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

        [HttpGet("osztalylistazasa")]
        public async Task<ActionResult<List<ClassDto>>> GetOsztalyok()
        {
            try
            {
                var result = await _model.GetOsztalyok();
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return NotFound("Nincs osztály az adatbázisban.");
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
