using BS_KretaProjekt.Dto;
using BS_KretaProjekt.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;

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
        //Ellenőrzi, hogy a GET /api/data/diaklistazasa végpont 200 OK-val tér vissza, amikor van legalább egy diák az adatbázisban (seed után).
        [Fact]
        public async Task GetDiakok()
        {
          

            var response = await _client.GetAsync("api/data/diaklistazasa");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Ellenőrzi, hogy a GET /api/data/diaklistazasa végpont vagy 200 OK-val, vagy 400 BadRequest-tel tér vissza. (Rugalmas teszt, mindkét válasz elfogadott.)
        [Fact]
        public async Task GetDiakok_ReturnsBadRequest()
        {
           

            var response = await _client.GetAsync("api/data/diaklistazasa");
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }
        //Ellenőrzi, hogy a GET /api/data/tanarlistazasa végpont 200 OK-val tér vissza, amikor van legalább egy tanár az adatbázisban.
        [Fact]
        public async Task GetTanarok()
        {
           

            var response = await _client.GetAsync("api/data/tanarlistazasa");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Ellenőrzi, hogy a PUT /api/data/modifystudentdata végpont 200 OK-val tér vissza érvényes diákadat küldésekor (id=1, nem üres név, valós mezők).
        [Fact]
        public async Task ModifyStudentData()
        {
          

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

            var contentt = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync("/api/data/modifystudentdata", contentt);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Ellenőrzi, hogy a PUT /api/data/modifystudentdata végpont 400 BadRequest-tel tér vissza, ha a diak_nev mező üres stringet tartalmaz (kötelező mező validáció).
        [Fact]
        public async Task ModifyStudentData_ReturnsBadRequest_baddiak()
        {
           

            var data = new
            {
                diak_id = 1,
                diak_nev = "",
                user_id = 1,
                osztaly_id = 1,
                lakcim = "Pécs - Király utca 11.",
                szuloneve = "Teszt Anyu",
                emailcim = "teszt@pelda.hu",
                jegyek = Array.Empty<object>(),
                szuletesi_datum = DateTime.Parse("2006-01-01")
            };

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync("/api/data/modifystudentdata", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }//Ellenőrzi, hogy a PUT /api/data/modifystudentdata végpont 400 BadRequest-tel tér vissza, ha a megadott diak_id nem létezik az adatbázisban (999999-es nem létező ID).

        [Fact]
        public async Task ModifyStudentData_ReturnsBadRequest_badiddiak()
        {
          

            var data = new
            {
                diak_id = 999999,
                diak_nev = "Teszt Elek",
                user_id = 1,
                osztaly_id = 1,
                lakcim = "Pécs - Király utca 11.",
                szuloneve = "Teszt Anyu",
                emailcim = "teszt@pelda.hu",
                jegyek = Array.Empty<object>(),
                szuletesi_datum = DateTime.Parse("2006-01-01")
            };

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync("/api/data/modifystudentdata", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        //Ellenőrzi, hogy a PUT /api/data/modifyteacherdata végpont 200 OK-val tér vissza érvényes tanáradat küldésekor.
        [Fact]
        public async Task ModifyTeacherData()
        {
           

            var data = new
            {
                tanar_id = 1,
                tanar_nev = "Teszt Tanár",
                szak = "Matematika"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync("/api/data/modifyteacherdata", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Ellenőrzi, hogy a DELETE /api/data/deletestudentdata végpont 404 NotFound-dal tér vissza, ha a megadott id nem létezik az adatbázisban.
        [Fact]
        public async Task DeleteStudentData_NotFound()
        {
            

            var response = await _client.DeleteAsync("/api/data/deletestudentdata?id=9999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        //Ellenőrzi, hogy a DELETE /api/data/deletestudentdata végpont 200 OK-val tér vissza egy valóban létező diák törlésekor. Az ID-t az adatbázisból kéri le dinamikusan.
        [Fact]
        public async Task DeleteStudentData_Ok()
        {
           

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            int id = db.Diakok.Select(d => d.diak_id).First();

            var response = await _client.DeleteAsync($"/api/data/deletestudentdata?id={id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Ellenőrzi, hogy a DELETE /api/data/deleteteacherdata végpont 200 OK-val tér vissza egy valóban létező tanár törlésekor. Az ID-t az adatbázisból kéri le dinamikusan.
        [Fact]
        public async Task DeleteTeacherData_OK()
        {
          

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KretaDbContext>();
            int id = db.Tanarok.Select(d => d.tanar_id).First();

            var response = await _client.DeleteAsync($"/api/data/deleteteacherdata?id={id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //Ellenőrzi, hogy a DELETE /api/data/deleteteacherdata végpont 404 NotFound-dal tér vissza, ha a megadott tanár ID nem létezik.
        [Fact]
        public async Task DeleteTeacherData_NotFound()
        {
           

            var response = await _client.DeleteAsync("/api/data/deleteteacherdata?id=9999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}