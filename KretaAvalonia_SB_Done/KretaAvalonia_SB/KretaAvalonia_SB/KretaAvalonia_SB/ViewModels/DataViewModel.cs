using Avalonia.Controls;
using KretaAvalonia_SB.Dto;
using KretaAvalonia_SB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace KretaAvalonia_SB.ViewModels
{
    public class DataViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly AuthModel _auth;
        private readonly GradeModel _grade;
        private readonly MessageModel _message;
        private readonly TimeTableModel _timetable;
        private readonly DataModel _data;

        private StudentDto? _studentData;
        public StudentDto? StudentData
        {
            get => _studentData;
            set
            {
                if (_studentData != value)
                {
                    _studentData = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand LoadDataCommand { get; }
        public ICommand BackCommand { get; }

        public DataViewModel(MainViewModel main, AuthModel auth, GradeModel grade, MessageModel message, TimeTableModel timetable, DataModel data)
        {
            _main = main;
            _auth = auth;
            _grade = grade;
            _message = message;
            _timetable = timetable;
            _data = data;

            LoadDataCommand = new  RelayCommand(async () => await LoadData());

            BackCommand = new  RelayCommand(() =>
            {
                _main.CurrentPage = new MenuViewModel(_main, _auth, _grade, _message, _timetable, _data);
             
            });

            _ = LoadData();
        }
        //Lekéri a bejelentkezett diák adatait és beállítja a StudentData property-t
        private async Task LoadData()
        {
            var userId = _auth.CurrentUserId;
            if (userId == 0) return;

            var result = await _data.GetMyStudentData(userId);
            if (result != null)
                StudentData = result;
        }
    }
}
