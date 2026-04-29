using CommunityToolkit.Mvvm.ComponentModel;
using KretaAvalonia_SB.Model;
using System;
using System.Threading.Tasks;

namespace KretaAvalonia_SB.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentPage;

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
    }

    private readonly AuthModel _auth;
    private readonly GradeModel _grade;
    private readonly MessageModel _message;
    private readonly TimeTableModel _timetable;
    private readonly DataModel _data;

    public MainViewModel(AuthModel auth, GradeModel grade, MessageModel message,TimeTableModel timetable, DataModel data)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _grade = grade ?? throw new ArgumentNullException(nameof(grade));
        _message = message ?? throw new ArgumentNullException(nameof(message));
        _timetable = timetable ?? throw new ArgumentNullException(nameof(timetable));
        _data = data ?? throw new ArgumentNullException(nameof(data));

        CurrentPage = new LoginViewModel(this, _auth, _grade, _message, _timetable, _data);
    }
}


