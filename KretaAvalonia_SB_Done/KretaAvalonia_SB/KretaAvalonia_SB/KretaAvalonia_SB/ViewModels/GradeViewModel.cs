using CommunityToolkit.Mvvm.Input;
using KretaAvalonia_SB.Dto;
using KretaAvalonia_SB.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KretaAvalonia_SB.ViewModels
{
    public class GradeViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly AuthModel _auth;
        private readonly GradeModel _grade;
        private readonly MessageModel _message;
        private readonly TimeTableModel _timetable;
        private readonly DataModel _data;

        public ObservableCollection<GradeDto> Grades { get; } = new();

        private GradeDto? _selectedGrade;
        public GradeDto? SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                if (_selectedGrade != value)
                {
                    _selectedGrade = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadGradesCommand { get; }
        public ICommand BackCommand { get; }

        public GradeViewModel(MainViewModel main, AuthModel auth, GradeModel grade,
            MessageModel message, TimeTableModel timetable, DataModel data)
        {
            _main = main;
            _auth = auth;
            _grade = grade;
            _message = message;
            _timetable = timetable;
            _data = data;

            LoadGradesCommand = new RelayCommand(async () => await LoadGrades());

            BackCommand = new RelayCommand(() =>
            {
                _main.CurrentPage = new MenuViewModel(_main, _auth, _grade, _message, _timetable, _data);
              
            });

            _ = LoadGrades();
        }

        private async Task LoadGrades()
        {
            var studentId = _auth.CurrentUserId;
            if (studentId == 0) return;

            var result = await _grade.GetMyGrade(studentId);
            Grades.Clear();
            if (result != null)
            {
                foreach (var grade in result)
                    Grades.Add(grade);
            }
        }
    }
}

