using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using KretaAvalonia.Model;

namespace KretaAvalonia.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {

        private object _currentView;
        public event EventHandler KijelentkezesEvent;
        public event EventHandler OrarendEvent;
        public event EventHandler UzenetEvent;
        public event EventHandler JegyekEvent;


        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string FelhasznaloNev { get; set; } = "felhasznalo neve";

        public RelayCommand OrarendCommand { get; }
        public RelayCommand UzenetekCommand { get; }
        public RelayCommand JegyekCommand { get; }
        public RelayCommand KijelentkezesCommand { get; }
        private ApiSession session;

        public MenuPageViewModel(ApiSession _session)
        {
            OrarendCommand = new RelayCommand(Orarendre);
            UzenetekCommand = new RelayCommand(Uzenetre);
            JegyekCommand = new RelayCommand(Jegyekre);
            KijelentkezesCommand = new RelayCommand(Kijelentkezes);
            session = _session;
            FelhasznaloNev = session.Username;
        }
        
        private void Kijelentkezes()
        {
            KijelentkezesEvent?.Invoke(this, EventArgs.Empty);
        }
        private void Orarendre()
        {
            OrarendEvent?.Invoke(this, EventArgs.Empty);
        }
        private void Uzenetre()
        {
            UzenetEvent?.Invoke(this, EventArgs.Empty);
        }
        private void Jegyekre()
        {
            JegyekEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}

