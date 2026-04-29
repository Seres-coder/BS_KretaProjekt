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

        //POST/api/message/messageadd – létrehoz és elment egy új üzenetet
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
        //GET/api/message/messageklistazasa – visszaadja egy felhasználó összes bejövő üzenetét
        [HttpGet("messageklistazasa")]
        public ActionResult<IEnumerable<MessageDto>> GetMessage([FromQuery] int user_id)
        {
            try
            {
                return Ok(_model.GetMessages(user_id));
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
        //GET/api/message/egymessagelistazasa – visszaad egyetlen üzenetet user és üzenet azonosító alapján
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
        //DELETE/api/message/deletemessage – törli a megadott üzenetet
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
