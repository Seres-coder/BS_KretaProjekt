using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KretaTest
{
    public class MessageModelTest
    {

        private readonly MessageModel _model;
        private readonly KretaDbContext _context;

        public MessageModelTest()
        {
            _context = DbContextFactory.Create();
            _model = new MessageModel(_context);
        }
        //Ellenőrzi, hogy a CreateMessage metódus sikeresen létrehoz egy új üzenetet, a rekordszám eggyel nő, és az üzenet adatai megegyeznek a DTO-val.
        [Fact]

        public async Task CreateMessage_Valid()
        {
            var before_count = _context.Uzenetek.Count();

            var dto = new BS_KretaProjekt.Dto.CreateMessageDto
            {
                cim = "Dolgozat",
                tartalom = "Holnap matematika dolgozat lesz.",
                fogado_id = 1,
                user_id = 2
            };

            await _model.CreateMessage(dto);
            var after_count = _context.Uzenetek.Count();
            Assert.Equal(before_count + 1, after_count);
            var created_message = _context.Uzenetek.OrderByDescending(x => x.uzenet_id).FirstOrDefault();
            Assert.NotNull(created_message);
            Assert.Equal(dto.cim, created_message.cim);
            Assert.Equal(dto.tartalom, created_message.tartalom);
            Assert.Equal(dto.fogado_id, created_message.fogado_id);
            Assert.Equal(dto.user_id, created_message.user_id);
        }
        //Ellenőrzi, hogy a CreateMessage metódus InvalidOperationException-t dob "Nincs minden adat megadva" üzenettel, ha a cím mező üres.
        [Fact]
        public async Task CreateMessage_ThrowsInvalidOperation()
        {
            var dto = new CreateMessageDto
            {
                cim = "", // <-- trigger (hiányos)
                tartalom = "Holnap matematika dolgozat lesz.",
                fogado_id = 1,
                user_id = 2
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _model.CreateMessage(dto));
            Assert.Equal("Nincs minden adat megadva", ex.Message);
        }


        //Ellenőrzi, hogy a GetMessages metódus KeyNotFoundException-t dob "nincs ilyen user" üzenettel, ha a megadott user_id nem létezik.
        [Fact]
        public void GetMessages_ThrowsKeyNotFound_WhenUserNotFound()
        {
            var nonExistingUserId = _context.Users.Any()
                ? _context.Users.Max(u => u.user_id) + 999
                : 999999;

            var ex = Assert.Throws<KeyNotFoundException>(() => _model.GetMessages(nonExistingUserId).ToList());
            Assert.Equal("nincs ilyen user", ex.Message);
        }

        //Ellenőrzi, hogy a GetOneMessage metódus visszaad egy létező üzenetet, amelynek van érvényes ID-je, neve, feladója és küldési időpontja.
        [Fact]
        public void GetOneMessage_Valid()
        {
            var user_id = 1;
            var message_id = _context.Uzenetek.Where(x => x.fogado_id == user_id).Select(x => x.uzenet_id).FirstOrDefault();
            var message = _model.GetOneMessage(user_id, message_id);
            Assert.NotNull(message);
            Assert.Equal(message_id, message.Id);
            Assert.False(string.IsNullOrWhiteSpace(message.cim));
            Assert.False(string.IsNullOrWhiteSpace(message.kuldoname));
            Assert.False(string.IsNullOrWhiteSpace(message.tartalom));
            Assert.True(message.kuldesidopontja <= DateTimeOffset.Now);
        }
        //Ellenőrzi, hogy a GetOneMessage metódus KeyNotFoundException-t dob "nincs ilyen user" üzenettel, ha a user_id nem létezik.
        [Fact]
        public void GetOneMessage_ThrowsKeyNotFound()
        {
            var nonExistingUserId = _context.Users.Any()
                ? _context.Users.Max(u => u.user_id) + 999
                : 999999;

            var existingMessageId = _context.Uzenetek.First().uzenet_id;

            var ex = Assert.Throws<KeyNotFoundException>(() =>
                _model.GetOneMessage(nonExistingUserId, existingMessageId));

            Assert.Equal("nincs ilyen user", ex.Message);
        }

        //Ellenőrzi, hogy a DeleteMessage metódus sikeresen törli az üzenetet: a rekordszám eggyel csökken, és az üzenet nem található többé.
        [Fact]
        public async Task DeleteMessage_Valid()
        {
            var user_id = 1;
            var message_id = _context.Uzenetek.Where(x => x.fogado_id == user_id).Select(x => x.uzenet_id).FirstOrDefault();
            var before_count = _context.Uzenetek.Count();
            await _model.DeleteMessage(user_id, message_id);
            var after_count = _context.Uzenetek.Count();
            Assert.Equal(before_count - 1, after_count);
            var deleted_message = _context.Uzenetek.Where(x => x.uzenet_id == message_id).FirstOrDefault();
            Assert.Null(deleted_message);
        }
        //Ellenőrzi, hogy a DeleteMessage metódus InvalidCastException-t dob "nincs ilyen uzenet" üzenettel, ha a megadott üzenet ID nem létezik.
        [Fact]
        public async Task DeleteMessage_ThrowsInvalidCastException_WhenMessageNotFound()
        {
            // olyan id, ami biztosan nem létezik
            var nonExistingMessageId = _context.Uzenetek.Any()
                ? _context.Uzenetek.Max(u => u.uzenet_id) + 999
                : 999999;

            var ex = await Assert.ThrowsAsync<InvalidCastException>(() => _model.DeleteMessage(nonExistingMessageId, 0));
            Assert.Equal("nincs ilyen uzenet", ex.Message);
        }
    }
}
