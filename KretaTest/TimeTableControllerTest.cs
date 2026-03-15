using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;

namespace KretaTest
{
    public class TimeTableControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly CustomApplicationFactory _factory;
        private readonly HttpClient _client;

        public TimeTableControllerTest(CustomApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
        }

        [Fact]
        public async Task AddTimeTable()
        {
            var responseAdmin = await _client.PostAsync(
                "api/user/login?username=admin&password=admin123",
                null);

            Assert.Equal(HttpStatusCode.OK, responseAdmin.StatusCode);
            responseAdmin.EnsureSuccessStatusCode();

            var contentAdmin = await responseAdmin.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(
                contentAdmin,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Admin", loginResult._Role);

            var data = new
            {
                osztaly_id = 1,
                nap = 1,
                ora = 1,
                tantargy = "Matematika",
                Tanarnev = "Kovács Tanár"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/timetable/orarendkrealas", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private int GetSeededOrarendId()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            return db.Orarendek.Select(j => j.orarend_id).First();
        }

        [Fact]
        public async Task ModifyTimeTable()
        {
            var responseAdmin = await _client.PostAsync(
                "api/user/login?username=admin&password=admin123",
                null);

            Assert.Equal(HttpStatusCode.OK, responseAdmin.StatusCode);
            responseAdmin.EnsureSuccessStatusCode();

            var contentAdmin = await responseAdmin.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(
                contentAdmin,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Admin", loginResult._Role);

            var orarendId = GetSeededOrarendId();

            var data = new
            {
                orarend_id = orarendId,
                osztaly_nev = "10.A",
                nap = 2,
                ora = 2,
                tantargy_nev = "Matematika",
                tanar_nev = "Kovács Tanár"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync("/api/timetable/modifytimetable", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ModifyTimeTable_ReturnsBadRequest_badorarend()
        {
            var login = await _client.PostAsync("api/user/login?username=tanar1&password=tanar123", null);
            login.EnsureSuccessStatusCode();

            var orarendId = GetSeededOrarendId();

            var data = new
            {
                orarend_id = orarendId,
                osztaly_nev = "10.A",
                nap = 2,
                ora = 0, // <-- trigger
                tantargy_nev = "Matematika",
                tanar_nev = "Kovács Tanár"
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/timetable/modifytimetable", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTimeTable()
        {
            var responseAdmin = await _client.PostAsync(
                "api/user/login?username=admin&password=admin123",
                null);

            Assert.Equal(HttpStatusCode.OK, responseAdmin.StatusCode);
            responseAdmin.EnsureSuccessStatusCode();

            var contentAdmin = await responseAdmin.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(
                contentAdmin,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Admin", loginResult._Role);

            var response = await _client.DeleteAsync("/api/timetable/deletetimetable?id=1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTimeTable()
        {
            var response = await _client.GetAsync("/api/timetable/gettimetable?osztaly_id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}