using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BS_KretaProjekt.Model
{
    public class MessageModel
    {
        private readonly KretaDbContext _context;
        public MessageModel(KretaDbContext context)
        {
            _context = context;
        }

        #region Create Message
        public async Task CreateMessage(CreateMessageDto dto)
        {
            using (var trx  = _context.Database.BeginTransaction())
            {
                _context.Uzenetek.Add(new Uzenet
                {
                    
                    cim = dto.cim,
                    tartalom = dto.tartalom,
                    fogado_id = dto.fogado_id,
                    user_id = dto.user_id,
                    kuldesidopontja=DateTimeOffset.Now,
                });
                await _context.SaveChangesAsync();
                await trx.CommitAsync();
            }
            await Task.CompletedTask;
            
        }
        #endregion

        #region Send Message
        public IEnumerable<MessageDto> GetMessages(int user_id)
        {
            return _context.Uzenetek.Include(x => x.User).Include(x => x.Fogado).Where(x => x.fogado_id == user_id).Select(x => new MessageDto
            {
                Id=x.uzenet_id,
                cim = x.cim,
                kuldoname = _context.Tanarok.Where(y => y.user_id == x.user_id).First().tanar_nev,
                tartalom = x.tartalom,
                kuldesidopontja=x.kuldesidopontja,
            }).OrderByDescending(x=>x.kuldesidopontja);
            
        }
        #endregion

        #region Get one message by id
        public MessageDto GetOneMessage(int user_id,int uzenet_id)
        {
            return _context.Uzenetek.Where(x => x.fogado_id == user_id && x.uzenet_id == uzenet_id).Select(x => new MessageDto
            {
                Id = x.uzenet_id,
                cim = x.cim,
                kuldoname = _context.Tanarok.Where(y => y.user_id == x.user_id).First().tanar_nev,
                tartalom = x.tartalom,
                kuldesidopontja = x.kuldesidopontja,
            }).First();
        }
        #endregion

        #region Delete Message
        public async Task DeleteMessage(int uzenet_id, int message_id)
        {
            if (!_context.Uzenetek.Any(x => x.uzenet_id == uzenet_id))
            {
                throw new InvalidCastException("nincs ilyen uzenet");
            }
            else
            {
                using (var trx = _context.Database.BeginTransaction())
                {
                    var uzenet = _context.Uzenetek.First(x => x.uzenet_id == uzenet_id);
                    _context.Uzenetek.Remove(uzenet);
                    await _context.SaveChangesAsync();
                    await trx.CommitAsync();
                }
            }
            await Task.CompletedTask;
        }
        #endregion

    }
}
