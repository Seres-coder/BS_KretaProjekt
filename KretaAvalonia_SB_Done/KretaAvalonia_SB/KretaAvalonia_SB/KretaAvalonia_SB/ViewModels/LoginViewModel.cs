using CommunityToolkit.Mvvm.Input;
using KretaAvalonia_SB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KretaAvalonia_SB.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        public string UserName
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private readonly MainViewModel _main;
        private readonly AuthModel _auth;
        private readonly GradeModel _grade;
        private readonly MessageModel _message;
        private readonly TimeTableModel _timetable;
        private readonly DataModel _data;
        public ICommand LoginCommand { get;}

        public LoginViewModel(MainViewModel main, AuthModel auth, GradeModel grade, MessageModel message, TimeTableModel timetable, DataModel data)
        {
            _main=main;
            _auth=auth;
            _grade = grade;
            _message = message;
            _timetable = timetable;
            _data = data;

            LoginCommand = new RelayCommand(async () => await LoggingIn());
        }

        private async Task LoggingIn()
        {
            ErrorMessage = string.Empty;

            var result = await _auth.Login(UserName, Password);
            if (result == null)
            {
                ErrorMessage = "Hibás felhasználónév vagy jelszó.";
                return;
            }

            _main.CurrentPage = new MenuViewModel(_main, _auth, _grade, _message, _timetable, _data);

        }
    }
}
