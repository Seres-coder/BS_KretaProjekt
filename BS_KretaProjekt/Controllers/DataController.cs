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
        //GET /api/data/getmydata – visszaadja a bejelentkezett diák saját adatait
        [HttpGet("mydata")]
        public async Task<ActionResult<StudentDto>> GetMyData([FromQuery] int userId)
        {
            try
            {
                var response = await _model.GetMyData(userId);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //GET /api/data/getmyteacherdata – visszaadja a bejelentkezett tanár saját adatait
        [HttpGet("myteacherdata")]
        public async Task<ActionResult<TeacherDto>> GetMyTeacherData([FromQuery] int userId)
        {
            try
            {
                var response = await _model.GetMyTeacherData(userId);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //GET /api/data/diaklistazasa – visszaadja az összes diák listáját
        [Authorize(Roles ="Admin")]
        [HttpGet("diaklistazasa")]
        public async Task <ActionResult<IEnumerable<StudentDto>>> GetDiak()
        {
            try
            {
                var response= await _model.GetDiak();
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        //GET /api/data/tanarlistazasa – visszaadja az összes tanár listáját
        [Authorize(Roles = "Admin")]
        [HttpGet("tanarlistazasa")]
        public async Task <ActionResult<IEnumerable<TeacherDto>>> GetTeacher()
        {
            try
            {
                var response = await _model.GetTeacher();
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
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
        //PUT /api/data/modifystudentdata – módosítja egy diák adatait
        [Authorize(Roles = "Admin")]
        [HttpPut("modifystudentdata")]
        public async Task<ActionResult> ModifyStudetData([FromBody] StudentDto dto)
        {
            try
            {
                await _model.ModifyStudentData(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
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
        //PUT /api/data/modifyteacherdata – módosítja egy tanár adatait
        [Authorize(Roles = "Admin")]
        [HttpPut("modifyteacherdata")]
        public async Task<ActionResult> ModifyTeacherData([FromBody] TeacherDto dto)
        {
            try
            {
                await _model.ModifyTeacherData( dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
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
        //DELETE /api/data/deletestudentdata – törli a megadott diákot
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
        //DELETE /api/data/deleteteacherdata – törli a megadott tanárt
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

        [HttpGet("tantargylistazasa")]
        public async Task<ActionResult> TantargyListazasa()
        {
            var list = await _model.TantargyListazasa();
            return Ok(list);
        }



    }
}
