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
    public class MessageViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly AuthModel _auth;
        private readonly GradeModel _grade;
        private readonly MessageModel _message;
        private readonly TimeTableModel _timetable;
        private readonly DataModel _data;

        public ObservableCollection<MessageDto> Messages { get; } = new();

        private MessageDto? _selectedMessage;
        public MessageDto? SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                if (_selectedMessage != value)
                {
                    _selectedMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadMessagesCommand { get; }
        public ICommand BackCommand { get; }

        public MessageViewModel(MainViewModel main, AuthModel auth, GradeModel grade,
            MessageModel message, TimeTableModel timetable, DataModel data)
        {
            _main = main;
            _auth = auth;
            _grade = grade;
            _message = message;
            _timetable = timetable;
            _data = data;

            LoadMessagesCommand = new  RelayCommand(async () => await LoadMessages());

            BackCommand = new  RelayCommand(() =>
            {
                _main.CurrentPage = new MenuViewModel(_main, _auth, _grade, _message, _timetable, _data);
     
            });

            _ = LoadMessages();
        }

        private async Task LoadMessages()
        {
            var studentId = _auth.CurrentUserId;
            if (studentId == 0) return;

            var result = await _message.GetMyMessage(studentId);
            Messages.Clear();
            if (result != null)
            {
                foreach (var msg in result)
                    Messages.Add(msg);
            }
        }
    }
}

