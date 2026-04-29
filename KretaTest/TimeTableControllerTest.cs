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
        //POST /api/timetable/orarendkrealas hívással új órarend bejegyzést ad hozzá érvényes adatokkal. Elvárás: 200 OK.
        [Fact]
        public async Task AddTimeTable()
        {
          

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
        //Bejelentkezik tanar1-ként, majd POST hívást küld ahol osztaly_id = 0 (érvénytelen). Elvárás: 400 BadRequest.
        [Fact]
        public async Task AddTimeTable_ReturnsBadRequest()
        {
            var login = await _client.PostAsync("api/user/login?username=tanar1&password=tanar123", null);
            login.EnsureSuccessStatusCode();

            var data = new
            {
                osztaly_id = 0, // <-- trigger
                nap = 1,
                ora = 1,
                tantargy = "Matematika",
                Tanarnev = "Kovács Tanár"
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/timetable/orarendkrealas", content);

            Assert.Equal((HttpStatusCode)406, response.StatusCode);
        }
        //Lekéri az első seed-elt órarend ID-ját közvetlenül az adatbázisból (DI scope-on keresztül), hogy ne kelljen fix ID-t hardcode-olni.
        private int GetSeededOrarendId()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            return db.Orarendek.Select(j => j.orarend_id).First();
        }
        //PUT /api/timetable/modifytimetable hívással módosítja az első seed-elt órarend bejegyzést. Elvárás: 200 OK.
        [Fact]
        public async Task ModifyTimeTable()
        {
           

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
        //Bejelentkezik tanar1-ként, majd PUT hívást küld ahol ora = 0 (érvénytelen). Elvárás: 400 BadRequest.
        [Fact]
        public async Task ModifyTimeTable_ReturnsBadRequest_missingdata()
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

            Assert.Equal((HttpStatusCode)406, response.StatusCode);
        }
        //Ugyanaz mint az előző – ellenőrzi, hogy érvénytelen ora esetén is 400 BadRequest érkezik.
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

            Assert.Equal((HttpStatusCode)406, response.StatusCode);
        }
        //DELETE /api/timetable/deletetimetable?id=1 hívással töröl egy órarend bejegyzést. Elvárás: 200 OK.
        [Fact]
        public async Task DeleteTimeTable()
        {
           

            var response = await _client.DeleteAsync("/api/timetable/deletetimetable?id=1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Bejelentkezik tanar1-ként, majd DELETE hívást küld nem létező ID-val (9999999). Elvárás: 400 BadRequest.
        [Fact]
        public async Task DeleteTimeTable_ReturnsBadRequest()
        {
            var login = await _client.PostAsync("api/user/login?username=tanar1&password=tanar123", null);
            login.EnsureSuccessStatusCode();

            var nonExistingOrarendId =9999999;

            var response = await _client.DeleteAsync($"/api/timetable/deletetimetable?id={nonExistingOrarendId}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        //GET /api/timetable/gettimetable?osztaly_id=1 lekéri a 10.A osztály órarendjét. Elvárás: 200 OK.
        [Fact]
        public async Task GetTimeTable()
        {
            var response = await _client.GetAsync("/api/timetable/gettimetable?osztaly_id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}