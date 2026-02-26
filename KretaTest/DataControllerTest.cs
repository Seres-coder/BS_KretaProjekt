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
                });

        }

        [Fact]
        public async Task GetDiakok()
        {
            var response = await _client.GetAsync("api/data/diaklistazasa");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetTanarok()
        {
            var response = await _client.GetAsync("api/data/tanarlistazasa");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ModifyStudentData()
        {

            var data = new
            {
                diak_id=1,
                diak_nev = "Teszt Elek",
                user_id = 1,
                osztaly_id = 1,
                lakcim = "Pécs - Király utca 11.",
                szuloneve = "Teszt Anyu",
                emailcim = "teszt@pelda.hu",
                jegyek = new object[] { },
                szuletesi_datum = DateTime.Parse("2006-01-01")
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/data/modifystudentdata", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task ModifyTeacherData()
        {
            var data = new
            {
                tanar_id = 1,
                tanar_nev = "Teszt Tanár",
                szak = "Matematika"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/data/modifyteacherdata", content);
        }
        /*
        [Fact]
        public async Task DeleteStudentData_OK()
        {
            var response = await _client.DeleteAsync("/api/data/deletestudentdata?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        */


    }
}
