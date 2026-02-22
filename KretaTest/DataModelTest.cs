using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;

namespace KretaTest
{
    public class DataModelTest
    {
        private readonly DataModel _model;
        private readonly KretaDbContext _context;

        public DataModelTest()
        {
            _context = DbContextFactory.Create();
            _model = new DataModel(_context);
        }

        [Fact]
        public void GetDiak_ReturnsStudents()
        {
            var result = _model.GetDiak();

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.All(result, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.diak_nev));
                Assert.True(x.user_id > 0);
                Assert.True(x.osztaly_id > 0);
                Assert.False(string.IsNullOrWhiteSpace(x.emailcim));
            });
        }

    }
}