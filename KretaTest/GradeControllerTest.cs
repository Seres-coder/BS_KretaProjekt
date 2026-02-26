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
        private readonly HttpClient _client;

        public GradeControllerTest(CustomApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }

        [Fact]
        public async Task AddNewGrade()
        {
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
        public async Task ModifyGrade()
        {
            var data = new
            {
                jegy_id = 1,
                tanar_nev = "Kovács Tanár",
                tantargy_nev = "Matematika",
                diak_nev = "Nagy Diák",
                ertek = 4
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/grade/grademodify", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task DeleteGrade()
        {
            var response = await _client.DeleteAsync("api/grade/gradedelete?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetAllGrades()
        {
            var response = await _client.GetAsync("api/grade/allgrade?id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
   

