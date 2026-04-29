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
        //POST /api/user/login helyes felhasználónévvel és jelszóval (diak1/diak123). Elvárás: 200 OK.
        [Fact]
        public async Task Login_OK()
        {
            var response = await _client.PostAsync("api/user/login?username=diak1&password=diak123", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //POST /api/user/login helyes felhasználónévvel, de rossz jelszóval. Elvárás: 401 Unauthorized.
        [Fact]
        public async Task Login_WrongPassword()
        {
            var response = await _client.PostAsync("api/user/login?username=diak1&password=rossz", null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        //POST /api/user/registration egyedi, véletlenszerű felhasználónévvel (Guid alapú). Elvárás: 200 OK.
        [Fact]
        public async Task Registration_OK()
        {
            var name = "tesztuser_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var response = await _client.PostAsync($"api/user/registration?name={name}&password=abc123", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        //POST /api/user/registration üres felhasználónévvel. Elvárás: 400 BadRequest.
        [Fact]
        public async Task Registration_BadRequest()
        {
            var response = await _client.PostAsync("api/user/registration?name=&password=abc123", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        //POST /api/user/registration már létező "admin" névvel. Elvárás: 409 Conflict
        [Fact]
        public async Task Registration_Conflict()
        {
            var response = await _client.PostAsync("api/user/registration?name=admin&password=abc123", null);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        //PUT /api/user/updatepassword üres jelszóval. Elvárás: 400 BadRequest.
        [Fact]
        public async Task UpdatePassword_BadRequest_WhenPasswordEmpty()
        {
            var response = await _client.PutAsync("api/user/updatepassword?userid=1&password=", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

      

     
    }
}

