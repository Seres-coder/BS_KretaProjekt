using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KretaTest
{
    public class UserModelTest
    {
        private readonly UserModel _model;
        private readonly KretaDbContext _context;

        public UserModelTest()
        {
            _context = DbContextFactory.Create();
            _model = new UserModel(_context);
        }

        [Fact]
        public async Task Registration()
        {
            int before = _context.Users.Count();
            int befored =  _context.Diakok.Count();
            await _model.Registration("Józsikaa", "asd");
            Assert.Equal(_context.Users.Count(), before+1);
            Assert.Equal(_context.Diakok.Count(), befored+1);
        }
        [Fact]
        public void Login()
        {
            UserDto User = _model.ValidateUser("admin", "admin123");
            Assert.Equal(User._Role, "Admin");
        }
        [Fact]
        public async Task PromoteTanar()
        {
            int before = _context.Users.Count();
            int beforet = _context.Tanarok.Count();
            int id = _context.Users.Where(x => x.belepesnev == "diak1").First().user_id;
            await _model.PromoteToTanar(id, "Matematika");
            Assert.Equal(_context.Users.Count(), before + 1);
            Assert.Equal(_context.Diakok.Count(), before - 1);
            Assert.Equal(_context.Tanarok.Count(), before + 1);
        }

    }
}
