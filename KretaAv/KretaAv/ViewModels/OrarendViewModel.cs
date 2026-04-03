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
    public class OrarendNapViewModel
    {
        public string NapNev { get; set; }
        public List<TimeTableItemDto> Orak { get; set; }

        public OrarendNapViewModel(string napNev, List<TimeTableItemDto> orak)
        {
            NapNev = napNev;
            Orak = orak;
        }
    }

    public class OrarendViewModel : ViewModelBase
    {
        private readonly ApiSession _session;
        private readonly OrarendModel _model;

        public ICommand VisszaCommand { get; }
        public event EventHandler VisszaEvent;

        public ObservableCollection<OrarendNapViewModel> Napok { get; set; } = new();

        private string _hibaUzenet = "";
        public string HibaUzenet
        {
            get => _hibaUzenet;
            set => SetProperty(ref _hibaUzenet, value);
        }

        public OrarendViewModel(ApiSession session, OrarendModel model)
        {
            VisszaCommand = new RelayCommand(Vissza);
            _session = session;
            _model = model;
        }

        private void Vissza()
        {
            VisszaEvent?.Invoke(this, EventArgs.Empty);
        }

        public async Task BetoltOrarend()
        {
            try
            {
                Napok.Clear();
                HibaUzenet = "";

                int osztalyId = await _model.GetOsztalyId();
                var orarend = await _model.GetOrarend(osztalyId);

               
                var napNevek = new Dictionary<int, string>
                {
                    { 1, "Hétfő"     },
                    { 2, "Kedd"      },
                    { 3, "Szerda"    },
                    { 4, "Csütörtök" },
                    { 5, "Péntek"    }
                };

                foreach (var nap in napNevek)
                {
                    if (orarend.TryGetValue(nap.Key, out var orak))
                        Napok.Add(new OrarendNapViewModel(nap.Value, orak));
                }

                if (Napok.Count == 0)
                    HibaUzenet = "Nincs órarend ehhez az osztályhoz.";
            }
            catch (Exception ex)
            {
                HibaUzenet = $"Hiba: {ex.Message}";
            }
        }
    }
}
