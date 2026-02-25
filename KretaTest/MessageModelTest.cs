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
        [Fact]
        public void GetMessages_Valid()
        {
            var user_id = 1;
            var messages = _model.GetMessages(user_id).ToList();
            Assert.NotNull(messages);
            Assert.NotEmpty(messages);
            Assert.All(messages, x =>
            {
                Assert.True(x.Id > 0);
                Assert.False(string.IsNullOrWhiteSpace(x.cim));
                Assert.False(string.IsNullOrWhiteSpace(x.kuldoname));
                Assert.False(string.IsNullOrWhiteSpace(x.tartalom));
                Assert.True(x.kuldesidopontja <= DateTime.Now);
            });
        }
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
    }
}
