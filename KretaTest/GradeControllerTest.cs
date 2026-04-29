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
        //Bejelentkezik tanar1-ként, majd POST /api/grade/gradeadd hívással hozzáad egy új 5-ös jegyet Nagy Diáknak Matematikából. Elvárás: 200 OK.
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
        //Bejelentkezik tanar1-ként, majd elküld egy jegyfelviteli kérést ahol az ertek = 0, ami érvénytelen. Elvárás: 400 BadRequest.
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

        //Lekéri az első seed-elt jegy ID-ját közvetlenül az adatbázisból(DI scope-on keresztül), hogy ne kelljen fix ID-t hardcode-olni.
        private int GetSeededJegyId()
        {


            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            return db.Jegyek.Select(j => j.jegy_id).First();
        }
        //Bejelentkezik tanar1-ként, majd PUT /api/grade/grademodify hívással módosítja az első seed-elt jegyet (4-esre állítja). Elvárás: 200 OK.
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
        //Bejelentkezik tanar1-ként, majd PUT /api/grade/grademodify hívást küld ahol az ertek = 0 (érvénytelen). Elvárás: 400 BadRequest.
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
        //Bejelentkezik tanar1-ként, majd DELETE /api/grade/gradedelete?id=1 hívással törli az id=1 jegyet. Elvárás: 200 OK.
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

        //Lekéri GET /api/grade/allgrade?id=1 végpontot (diák ID=1 jegyei). Elvárás: 200 OK.
        [Fact]
        public async Task GetAllGrades()
        {
            var response = await _client.GetAsync("api/grade/allgrade?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Lekéri GET /api/grade/allgrade?id= végpontot üres ID-val. Elvárás: 400 BadRequest (hiányzó kötelező paraméter).
        [Fact]
        public async Task AllGrades_ReturnsNotFound()
        {
            var response = await _client.GetAsync("api/grade/allgrade?id=");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
   

