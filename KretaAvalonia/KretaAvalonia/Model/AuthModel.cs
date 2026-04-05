using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace KretaAvalonia.Model
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
            // A backend [FromQuery]-t vár, nem [FromBody]-t!
            var res = await _session.Client.PostAsync(
                $"api/User/login?username={Uri.EscapeDataString(dto._belepesnev)}&password={Uri.EscapeDataString(dto._jelszo)}",
                null);

            if (!res.IsSuccessStatusCode)
                return res;

            // A login válasza már tartalmazza az adatokat: { id, name, role }
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

        // Belső DTO a login válaszhoz: { id, name, role }
        private class LoginResponseDto
        {
            public int id { get; set; }
            public string name { get; set; } = "";
            public string role { get; set; } = "";
        }
    }
}