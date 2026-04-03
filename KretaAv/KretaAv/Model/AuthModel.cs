using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using KretaAv.Dto;


namespace KretaAv.Model
{
    public class AuthModel
    {
        public ApiSession _session;

        public AuthModel(ApiSession session)
        {
            _session = session;
        }

        public async Task<HttpResponseMessage> Login(Dto.UserDto dto)
        {
           
            var res = await _session.Client.PostAsync(
                $"api/User/login?username={Uri.EscapeDataString(dto._belepesnev)}&password={Uri.EscapeDataString(dto._jelszo)}",
                null);

            if (!res.IsSuccessStatusCode)
                return res;

            
            var user = await res.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (user == null)
                throw new InvalidOperationException("Nem sikerült beolvasni a bejelentkezési választ.");

            _session.Userid = user.id;
            _session.Username = user.name;
            _session.Role = user.role;

            return res;
        }

        public async Task Logout()
        {
            var res = await _session.Client.PostAsync("api/User/logout", null);
            res.EnsureSuccessStatusCode();

            _session.Userid = 0;
            _session.Username = "";
            _session.Role = "";
        }

        
    }
}
