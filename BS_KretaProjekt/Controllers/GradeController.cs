using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BS_KretaProjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private GradeModel _model;
        public GradeController(GradeModel model)
        {
            _model = model;
        }

        #region Grade Add
        //POST /api/grade/gradeadd – új jegyet ad hozzá egy diáknak
        [Authorize(Roles = "Tanar")]
        [HttpPost("gradeadd")]
        public async Task<ActionResult> AddNewGrade([FromBody] GradeAdd dto)
        {
            try
            {
                await _model.AddNewGrade(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                { return StatusCode(406, ex.Message); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); // ← teljes stack trace
                return BadRequest(ex.Message + " | Inner: " + ex.InnerException?.Message);
            }
        }
        #endregion

        #region -Grade Modify
        //PUT /api/grade/grademodify – módosítja egy meglévő jegy értékét
        [Authorize(Roles = "Tanar")]
        [HttpPut("grademodify")]
        public async Task<ActionResult> ModifyGrade([FromBody] GradeModify dto)
        {
            try
            {
                await _model.GradeModify( dto);
                return Ok();
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
        #endregion

        #region -Grade Delete
        //DELETE /api/grade/gradedelete – törli a megadott azonosítójú jegyet
        [Authorize(Roles = "Tanar")]
        [HttpDelete("gradedelete")]
        public async Task<ActionResult> DeleteGrade([FromQuery] int id)
        {
            try
            {
               await _model.DeleteGrade(id);
                return Ok();
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
        #endregion

        #region -Grade Listing
        //GET /api/grade/allgrade – visszaadja egy diák vagy tanár összes jegyét id alapján
        [HttpGet("allgrade")]
        public ActionResult<IEnumerable<GradeListDto>> GetAllGrades([FromQuery] int id = 0, [FromQuery] int tanar_id = 0)
        {
            try
            {
                return Ok(_model.AllGrades(id, tanar_id));
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
        #endregion
    }
}
