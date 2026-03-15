using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BS_KretaProjekt.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using BS_KretaProjekt.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace KretaTest
{
    public class UserControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly HttpClient _client;

        public UserControllerTest(CustomApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
        }

        [Fact]
        public async Task Login_OK()
        {
            var response = await _client.PostAsync("api/user/login?username=diak1&password=diak123", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_WrongPassword()
        {
            var response = await _client.PostAsync("api/user/login?username=diak1&password=rossz", null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Registration_OK()
        {
            var name = "tesztuser_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var response = await _client.PostAsync($"api/user/registration?name={name}&password=abc123", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Registration_BadRequest()
        {
            var response = await _client.PostAsync("api/user/registration?name=&password=abc123", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Registration_Conflict()
        {
            var response = await _client.PostAsync("api/user/registration?name=admin&password=abc123", null);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePassword_BadRequest_WhenPasswordEmpty()
        {
            var response = await _client.PutAsync("api/user/updatepassword?userid=1&password=", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRole_Unauthorized_WhenNotLoggedIn()
        {
            var response = await _client.PutAsync("api/user/upgraderole?id=1&tantargy=Matematika", null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

     
    }
}

