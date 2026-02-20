using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
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
        [HttpPost("gradeadd")]
        public async Task<ActionResult> AddNewGrade([FromBody] GradeAdd dto)
        {
            try
            {
                await _model.AddNewGrade(dto);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        #endregion

        #region -Grade Modify

        #endregion

        #region -Grade Delete
        [HttpDelete("gradedelete")]
        public async Task<ActionResult> DeleteGrade([FromQuery] int id)
        {
            try
            {
               await _model.DeleteGrade(id);
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
        #endregion

        #region -Grade Listing

        [HttpGet("allgrade")]
        public ActionResult<IEnumerable<GradeListDto>> GetAllGrades([FromQuery] int id)
        {
            try
            {
                return Ok(_model.AllGrades(id));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        #endregion
    }
}
