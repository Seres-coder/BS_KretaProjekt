using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BS_KretaProjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {
        private TimeTableModel _model;
        public TimeTableController(TimeTableModel model)
        {
            _model = model;
        }
        //POST /api/timetable/orarendkrealas – létrehoz egy új órarendi bejegyzést
        [Authorize(Roles = "Admin")]
        [HttpPost("orarendkrealas")]
        public async Task<ActionResult> AddTimeTabel([FromBody] CreateOrarendDto dto)
        {
            try
            {
                await _model.CreateTimeTable(dto);
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
        //PUT /api/timetable/modifytimetable – módosítja a megadott órarendi bejegyzést
        [Authorize(Roles = "Admin")]
        [HttpPut("modifytimetable")]
        public async Task<ActionResult> ModifyTimeTable([FromBody] UpdateOrarendDto dto)
        {
            try
            {
                await _model.ModifyTimeTable(dto);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (InvalidCastException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        // DELETE /api/timetable/deletetimetable – törli a megadott órarendi bejegyzést
        [Authorize(Roles = "Admin")]
        [HttpDelete("deletetimetable")]
        public async Task<ActionResult> DeleteTimeTable([FromQuery] int id)
        {
            try
            {
                await _model.DeleteTimeTable(id);
                return Ok();
            }
            catch (InvalidCastException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //GET /api/timetable/gettimetable – visszaadja egy osztály órarendjét napok szerint csoportosítva
        [HttpGet("gettimetable")]
        public ActionResult<Dictionary<DayOfWeek, List<TimeTableItemDto>>> GetTimeTable([FromQuery] int osztaly_id)
        {
            try
            {
                var result = _model.GetTimeTable(osztaly_id);

                if (result == null || result.Count == 0)
                    return NotFound("Nincs órarend az adott osztályhoz.");

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        // GET /api/timetable/teachertimetabel – visszaadja egy tanár órarendjét napok szerint csoportosítva
        [HttpGet("teachertimetabel")]
        public ActionResult<Dictionary<DayOfWeek, List<TeacherTimeTabelDto>>> GetTeacherTimeTable([FromQuery] int tanarId)
        {
            try
            {
                var response = _model.GetTeacherTimeTable(tanarId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Szerverhiba: {ex.Message}");
            }
        }

    }
}
