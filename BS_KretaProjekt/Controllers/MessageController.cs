using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BS_KretaProjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private MessageModel _model;
        public MessageController(MessageModel model)
        {
            _model = model;
        }


        [HttpPost("messageadd")]
        public async Task<ActionResult> AddNewMessage([FromBody] CreateMessageDto dto)
        {
            try
            {
                await _model.CreateMessage(dto);
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

        [HttpGet("messageklistazasa")]
        public ActionResult<IEnumerable<MessageDto>> GetMessage([FromQuery] int fogado_id)
        {
            try
            {
                return Ok(_model.GetMessages(fogado_id));
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

        [HttpGet("egymessagelistazasa")]
        public ActionResult<IEnumerable<MessageDto>> GetOneMessage([FromQuery] int user_id, [FromQuery]  int uzenet_id)
        {
            try
            {
                return Ok(_model.GetOneMessage(user_id,uzenet_id));
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
        [HttpDelete("deletemessage")]
        public async Task<ActionResult> DeleteMessage([FromQuery] int id, [FromQuery] int message_id)
        {
            try
            {
                await _model.DeleteMessage(id,message_id);
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

    }
}
