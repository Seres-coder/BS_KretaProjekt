using CommunityToolkit.Mvvm.Input;
using KretaAvalonia_SB.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KretaAvalonia_SB.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly AuthModel _auth;
        private readonly GradeModel _grade;
        private readonly MessageModel _message;
        private readonly TimeTableModel _timetable;
        private readonly DataModel _data;

        public ICommand DataCommand { get; }
        public ICommand TimeTableCommand { get; }
        public ICommand GradeCommand { get; }
        public ICommand MessageCommand { get; }
        public ICommand LogOutCommand { get; }

        public MenuViewModel(MainViewModel main, AuthModel auth, GradeModel grade,
            MessageModel message, TimeTableModel timetable, DataModel data)
        {
            _main = main;
            _auth = auth;
            _grade = grade;
            _message = message;
            _timetable = timetable;
            _data = data;

            DataCommand = new RelayCommand(() =>
            {
                _main.CurrentPage = new DataViewModel(_main, _auth, _grade, _message, _timetable, _data);
    
            });

            TimeTableCommand = new RelayCommand(async () =>
            {
                if (_auth.CurrentClassId == 0)
                    await LoadStudentClassId();

                var classId = _auth.CurrentClassId;
                _main.CurrentPage = new TimeTableViewModel(_main, _auth, _grade, _message, _timetable, _data, classId);
            });

            GradeCommand = new RelayCommand(() =>
            {
                _main.CurrentPage = new GradeViewModel(_main, _auth, _grade, _message, _timetable, _data);
           
            });

            MessageCommand = new RelayCommand(() =>
            {
                _main.CurrentPage = new MessageViewModel(_main, _auth, _grade, _message, _timetable, _data);
           
            });

            LogOutCommand = new RelayCommand(() =>
            {
                _auth.Logout();
                _main.CurrentPage = new LoginViewModel(_main, _auth, _grade, _message, _timetable, _data);

            });

            _ = LoadStudentClassId();
        }
        private async Task LoadStudentClassId()
        {
            var userId = _auth.CurrentUserId;
            if (userId == 0) return;

            var result = await _data.GetMyStudentData(userId);
            if (result != null)
                _auth.CurrentClassId = result.osztaly_id ?? 0;
        }
    }
}