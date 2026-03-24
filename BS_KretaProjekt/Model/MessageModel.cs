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
            if(string.IsNullOrWhiteSpace(dto.cim) || string.IsNullOrWhiteSpace(dto.tartalom) || dto.fogado_id == 0 || dto.user_id == 0)
                throw new InvalidOperationException("Nincs minden adat megadva");

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
        public IEnumerable<MessageDto> GetMessages(int fogado_id)
        {
            if (!_context.Diakok.Any(x => x.diak_id == fogado_id))
                throw new KeyNotFoundException("nincs ilyen diák");

            return _context.Uzenetek
                .Include(x => x.User)
                .Include(x => x.Fogado)
                .Where(x => x.fogado_id == fogado_id)
                .Select(x => new MessageDto
                {
                    Id = x.uzenet_id,
                    cim = x.cim,
                    kuldoname =
                        _context.Tanarok
                            .Where(t => t.user_id == x.user_id)
                            .Select(t => t.tanar_nev)
                            .FirstOrDefault()
                        ?? _context.Diakok
                            .Where(d => d.user_id == x.user_id)
                            .Select(d => d.diak_nev)
                            .FirstOrDefault()
                        ?? _context.Users
                            .Where(u => u.user_id == x.user_id)
                            .Select(u => u.belepesnev)
                            .FirstOrDefault()
                        ?? "Ismeretlen feladó",
                    tartalom = x.tartalom,
                    kuldesidopontja = x.kuldesidopontja
                })
                .OrderByDescending(x => x.kuldesidopontja)
                .ToList();
        }
        #endregion

        #region Get one message by id
        public MessageDto GetOneMessage(int user_id,int uzenet_id)
        {
            if (!_context.Users.Any(x => x.user_id == user_id))
                throw new KeyNotFoundException("nincs ilyen user");

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
