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
    public class MessageControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly HttpClient _client;
        public MessageControllerTest(CustomApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }
        //POST /api/message/messageadd hívással új üzenetet küld érvényes adatokkal. Elvárás: 200 OK.
        [Fact]
        public async Task AddNewMessage_OK()
        {
            var data = new
            {
                cim = "Teszt üzenet",
                tartalom = "Ez egy teszt tartalom.",
                fogado_id = 1,
                user_id = 1
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/message/messageadd", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //POST /api/message/messageadd hívással üres cím mezőt küld. Elvárás: 400 BadRequest.
        [Fact]
        public async Task AddNewMessage_ReturnsBadRequest()
        {
            var data = new
            {
                cim = "", // <-- trigger
                tartalom = "Ez egy teszt tartalom.",
                fogado_id = 1,
                user_id = 1
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/message/messageadd", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        //GET /api/message/messageklistazasa nem létező user_id-val (999999). Elvárás: 404 NotFound.
        [Fact]
     
    
        public async Task GetMessage_ReturnsNotFound()
        {
            var nonExistingUserId = 999999;

            var response = await _client.GetAsync($"/api/message/messageklistazasa?user_id={nonExistingUserId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        //GET /api/message/egymessagelistazasa létező adatokkal (user_id=1, uzenet_id=1). Elvárás: 200 OK.
        [Fact]
        public async Task GetOneMessage()
        {
            var response = await _client.GetAsync("/api/message/egymessagelistazasa?user_id=1&uzenet_id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //GET /api/message/egymessagelistazasa nem létező user_id-val. Elvárás: 404 NotFound.
        [Fact]
        public async Task GetOneMessage_ReturnsNotFound()
        {
            var nonExistingUserId = 999999;

            var response = await _client.GetAsync($"/api/message/egymessagelistazasa?user_id={nonExistingUserId}&uzenet_id=1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        //DELETE /api/message/deletemessage?id=1&message_id=1 hívással töröl egy létező üzenetet. Elvárás: 200 OK.
        [Fact]
        public async Task DeleteMessage()
        {
            var response = await _client.DeleteAsync("/api/message/deletemessage?id=1&message_id=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //DELETE /api/message/deletemessage nem létező user ID-val. Elvárás: 404 NotFound.
        [Fact]
        public async Task DeleteMessage_ReturnsNotFound()
        {
            var nonExistingMessageId = 999999;

            var response = await _client.DeleteAsync($"/api/message/deletemessage?id={nonExistingMessageId}&message_id=1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
