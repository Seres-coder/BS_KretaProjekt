using BS_KretaProjekt.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KretaTest
{
    public class TimeTableControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly HttpClient _client;
        public TimeTableControllerTest(CustomApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }
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
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/timetable/orarendkrealas", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task ModifyTimeTable()
        {
            var data = new
            {
                orarend_id = 1,
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
        public async Task DeleteTimeTable()
        {
            var response = await _client.DeleteAsync("/api/timetable/deletetimetable?id=1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetTimeTable()
        {
            var response = await _client.GetAsync("/api/timetable/gettimetable?osztaly_id=1");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
