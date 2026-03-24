using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KretaTest
{
    public class GradeControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly CustomApplicationFactory _factory;
        private readonly HttpClient _client;

        public GradeControllerTest(CustomApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
        }

        [Fact]
        public async Task AddNewGrade()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var data = new
            {
                tanar_nev = "Kovács Tanár",
                tantargy_nev = "Matematika",
                diak_nev = "Nagy Diák",
                ertek = 5
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/grade/gradeadd", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task AddNewGrade_ReturnsBadRequest()
        {
            var login = await _client.PostAsync("api/user/login?username=tanar1&password=tanar123", null);
            login.EnsureSuccessStatusCode();

            var data = new
            {
                tanar_nev = "Kovács Tanár",
                tantargy_nev = "Matematika",
                diak_nev = "Nagy Diák",
                ertek = 0 // <-- trigger
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/grade/gradeadd", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //scope mivel eddig mukododtt mostmeg mar for some reason nem?
        private int GetSeededJegyId()
        {


            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            return db.Jegyek.Select(j => j.jegy_id).First();
        }
        [Fact]
        public async Task ModifyGrade()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var jegyId = GetSeededJegyId();
            var data = new
            {
                jegy_id = jegyId,
                tanar_nev = "Kovács Tanár",
                tantargy_nev = "Matematika",
                diak_nev = "Nagy Diák",
                ertek = 4,
                updatedatum = DateTimeOffset.UtcNow
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/grade/grademodify", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task ModifyGrade_ReturnsBadRequest_WhenMissingData()
        {
            var login = await _client.PostAsync("api/user/login?username=tanar1&password=tanar123", null);
            login.EnsureSuccessStatusCode();

            var jegyId = GetSeededJegyId();

            var data = new
            {
                jegy_id = jegyId,
                ertek = 0, // <-- trigger
                updatedatum = DateTimeOffset.UtcNow
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/grade/grademodify", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGrade()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.DeleteAsync("api/grade/gradedelete?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetAllGrades()
        {
            var response = await _client.GetAsync("api/grade/allgrade?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task AllGrades_ReturnsNotFound()
        {
            var response = await _client.GetAsync("api/grade/allgrade?id=");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
   

