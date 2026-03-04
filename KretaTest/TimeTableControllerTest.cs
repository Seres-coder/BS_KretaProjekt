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
                osztaly_id = 1,
                nap = 1,
                ora = 1,
                tantargy = "Matematika",
                Tanarnev = "Kovács Tanár"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/timetable/orarendkrealas", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

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

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var orarendid = GetSeededOrarendId();
            var data = new
            {
                orarend_id = orarendid,
                osztaly_nev = "10.A",
                nap = 2,
                ora = 2,
                tantargy_nev = "Matematika",
                tanar_nev = "Kovács Tanár"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/timetable/modifytimetable", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

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

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.DeleteAsync("/api/timetable/deletetimetable?id=1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTimeTable_ReturnsBadRequest()
        {
            var login = await _client.PostAsync("api/user/login?username=tanar1&password=tanar123", null);
            login.EnsureSuccessStatusCode();

            var nonExistingOrarendId =9999999;

            var response = await _client.DeleteAsync($"/api/timetable/deletetimetable?id={nonExistingOrarendId}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetTimeTable()
        {
            var response = await _client.GetAsync("/api/timetable/gettimetable?osztaly_id=1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
