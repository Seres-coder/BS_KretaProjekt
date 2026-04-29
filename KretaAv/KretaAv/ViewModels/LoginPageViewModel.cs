using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using KretaAv.Model;

namespace KretaAv.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private AuthModel model;

        public string UserName
        {
            get => _username;
            set { if (value != null) { _username = value; } OnPropertyChanged(nameof(UserName)); }
        }
        private string _username = "";

        public string Password
        {
            get => _password;
            set { if (value != null) { _password = value; } OnPropertyChanged(nameof(Password)); }
        }
        private string _password = "";

        public ICommand BelepesCommand { get; set; }
        public event EventHandler SikeresBelepes;

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public LoginPageViewModel(AuthModel model)
        {
            BelepesCommand = new AsyncRelayCommand(CheckLogin);
            this.model = model;
        }

        public async Task CheckLogin()
        {
            try
            {
                Message = "Bejelentkezés...";
                await Login();
                SikeresBelepes?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
               
                Message = $"Hiba: {e.Message}";
                if (e.InnerException != null)
                    Message += $" | {e.InnerException.Message}";
            }
        }

        private async Task Login()
        {
            var data = new Dto.UserDto
            {
                _belepesnev = UserName,
                _jelszo = Password,
                _Role = "Diak"
            };
            var response = await model.Login(data);
            response.EnsureSuccessStatusCode();
        }
    }
}
