using KretaAvalonia_SB.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KretaAvalonia_SB.Model
{
    public class AuthModel
    {
        private readonly HttpClient _client;

        public AuthModel(HttpClient client)
        {
            _client = client;
        }

        public LoginResDto? CurrentUser { get; private set; }
        public int CurrentUserId => CurrentUser?.UserId ?? 0;
        public int CurrentClassId { get; set; } = 0;
        public bool IsLoggedIn => CurrentUser != null;

        //Bejelentkezési kérést küld a szervernek, sikeres válasz esetén elmenti a bejelentkezett usert
        public async Task<LoginResDto?> Login(string username, string password)
        {
            var endpoint = "api/User/login";
            var url = $"{endpoint}?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";
            var response = await _client.PostAsync(url, null);
            if (!response.IsSuccessStatusCode)
                return null;

            CurrentUser = await response.Content.ReadFromJsonAsync<LoginResDto>();
            return CurrentUser;
        }
        //Kijelentkeztet: törli az eltárolt bejelentkezett user adatait
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
