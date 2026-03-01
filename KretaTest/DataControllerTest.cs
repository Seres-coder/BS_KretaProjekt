using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KretaTest
{
    public class DataControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomApplicationFactory _factory;
        public DataControllerTest(CustomApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                    HandleCookies = true
                });

        }

        [Fact]
        public async Task GetDiakok()
        {
            var responseTanar = await _client.PostAsync(
            "api/user/login?username=tanar1&password=tanar123",
            null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var content = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Tanar", loginResult._Role);

            var response = await _client.GetAsync("api/data/diaklistazasa");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetTanarok()
        {
            var responseTanar = await _client.PostAsync(
            "api/user/login?username=tanar1&password=tanar123",
            null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var content = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Tanar", loginResult._Role);

            var response = await _client.GetAsync("api/data/tanarlistazasa");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ModifyStudentData()
        {
            var responseTanar = await _client.PostAsync(
            "api/user/login?username=tanar1&password=tanar123",
            null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var content = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Tanar", loginResult._Role);

            var data = new
            {
                diak_id = 1,
                diak_nev = "Teszt Elek",
                user_id = 1,
                osztaly_id = 1,
                lakcim = "Pécs - Király utca 11.",
                szuloneve = "Teszt Anyu",
                emailcim = "teszt@pelda.hu",
                jegyek = new object[] { },
                szuletesi_datum = DateTime.Parse("2006-01-01")
            };

            var contentt = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/data/modifystudentdata", contentt);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task ModifyTeacherData()
        {
            var responseTanar = await _client.PostAsync(
            "api/user/login?username=tanar1&password=tanar123",
            null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal("Tanar", loginResult._Role);

            var data = new
            {
                tanar_id = 1,
                tanar_nev = "Teszt Tanár",
                szak = "Matematika"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/data/modifyteacherdata", content);
        }

        [Fact]
        public async Task DeleteStudentData_NotFound()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.DeleteAsync("/api/data/deletestudentdata?id=9999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task DeleteStudentData_Ok()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            int id = db.Diakok.Select(d => d.diak_id).First();

            var response = await _client.DeleteAsync($"/api/data/deletestudentdata?id={id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTeacherData_OK()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            int id = db.Tanarok.Select(d => d.tanar_id).First();
            var response = await _client.DeleteAsync($"/api/data/deleteteacherdata?id={id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTeacherData_NotFound()
        {
            var responseTanar = await _client.PostAsync(
           "api/user/login?username=tanar1&password=tanar123",
           null);
            Assert.Equal(HttpStatusCode.OK, responseTanar.StatusCode);
            responseTanar.EnsureSuccessStatusCode();
            var contenttanar = await responseTanar.Content.ReadAsStringAsync();

            var loginResult = JsonSerializer.Deserialize<UserDto>(contenttanar,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.DeleteAsync("/api/data/deleteteacherdata?id=9999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}
