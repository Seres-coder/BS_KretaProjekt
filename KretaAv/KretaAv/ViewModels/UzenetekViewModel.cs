using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using KretaAv.Dto;
using KretaAv.Model;

namespace KretaAv.ViewModels
{
    public class UzenetekViewModel : ViewModelBase
    {
        private readonly UzenetekModel _model;

        public ICommand VissszaCommand { get; }
        public event EventHandler? VissszaEvent;

        public ObservableCollection<UzenetekDto> Uzenetek { get; } = new();

        private string _hibaUzenet = "";
        public string HibaUzenet
        {
            get => _hibaUzenet;
            set => SetProperty(ref _hibaUzenet, value);
        }

        public UzenetekViewModel(ApiSession session)
        {
            _model = new UzenetekModel(session);
            VissszaCommand = new RelayCommand(Vissza);

            Task.Run(async () => await BetoltUzenetek());
        }

        private void Vissza()
        {
            VissszaEvent?.Invoke(this, EventArgs.Empty);
        }

        public async Task BetoltUzenetek()
        {
            try
            {
                Uzenetek.Clear();
                HibaUzenet = "";

                var uzenetek = await _model.GetUzenetek();

                if (uzenetek == null || uzenetek.Count == 0)
                {
                    HibaUzenet = "Nincsenek üzenetek.";
                    return;
                }

                foreach (var uzenet in uzenetek)
                {
                    Uzenetek.Add(uzenet);
                }
            }
            catch (Exception ex)
            {
                HibaUzenet = $"Hiba: {ex.Message}";
            }
        }
    }
}
