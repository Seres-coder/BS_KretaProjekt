using KretaAvalonia_SB.Dto;
using KretaAvalonia_SB.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace KretaAvalonia_SB.ViewModels
{
    public class TimeTableViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly AuthModel _auth;
        private readonly GradeModel _grade;
        private readonly MessageModel _message;
        private readonly TimeTableModel _timetable;
        private readonly DataModel _data;
        private readonly int _classId;

        private List<TimeTableDto> _allEntries = new();
        public ObservableCollection<TimeTableDto> FilteredTimeTable { get; } = new();

        private int _selectedDay = 1;
        public int SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged();
                    FilterByDay();
                }
            }
        }

        public ICommand LoadTimeTableCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SelectDayCommand { get; }

        public TimeTableViewModel(MainViewModel main, AuthModel auth, GradeModel grade,
            MessageModel message, TimeTableModel timetable, DataModel data, int classId)
        {
            _main = main;
            _auth = auth;
            _grade = grade;
            _message = message;
            _timetable = timetable;
            _data = data;
            _classId = classId;

            LoadTimeTableCommand = new  RelayCommand(async () => await LoadTimeTable());

            BackCommand = new  RelayCommand(() =>
            {
                _main.CurrentPage = new MenuViewModel(_main, _auth, _grade, _message, _timetable, _data);
             
            });

            SelectDayCommand = new  RelayCommand<string>(param =>
            {
                if (param is string s && int.TryParse(s, out int day))
                    SelectedDay = day;

            });

            _ = LoadTimeTable();
        }

        private TimeTableResponseDto? _allData;
        //Lekéri az osztály teljes órarendjét a szervertől, majd szűri az aktuális napra
        private async Task LoadTimeTable()
        {
            if (_classId == 0) return;
            var result = await _timetable.GetTimeTable(_classId);
            if (result != null)
            {
                _allData = result;
                FilterByDay();
            }
        }
        //A kiválasztott nap alapján szűri az órarendet és frissíti a FilteredTimeTable kollekcióba
        private void FilterByDay()
        {
            FilteredTimeTable.Clear();
            var list = SelectedDay switch
            {
                1 => _allData?.Monday,
                2 => _allData?.Tuesday,
                3 => _allData?.Wednesday,
                4 => _allData?.Thursday,
                5 => _allData?.Friday,
                _ => null
            };
            if (list != null)
                foreach (var item in list)
                    FilteredTimeTable.Add(item);
        }
    }
}

