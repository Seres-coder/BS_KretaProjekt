using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BS_KretaProjekt.Dto;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KretaTest
{
    public class UserControllerTest : IClassFixture<CustomApplicationFactory>
    {
        private readonly HttpClient _client;

        public UserControllerTest(CustomApplicationFactory factory)
        {
            // Kell, hogy a cookie-kat kezelje (mert login SignInAsync cookie-t állíthat be)
            _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
        }
        [Fact]
        public async Task Login_OK()
        {
            var response = await _client.PostAsync("api/user/login?username=diak1&password=diak123", content: null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
    
