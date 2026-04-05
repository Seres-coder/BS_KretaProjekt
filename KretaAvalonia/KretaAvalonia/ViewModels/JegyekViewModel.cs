using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Input;
using KretaAvalonia.Dto;
using KretaAvalonia.Model;

namespace KretaAvalonia.ViewModels
{
    public class JegyekViewModel
    {
        public JegyekModel model;
        public JegyekViewModel(JegyekModel _model)
        {
            Tantargyak = new ObservableCollection<TantargyatlagViewModel>();
            VisszaCommand = new RelayCommand(Vissza);
            model = _model;

        }
        public ICommand VisszaCommand { get; }
        public event EventHandler VisszaEvent;
        private void Vissza()
        {
            VisszaEvent?.Invoke(this, EventArgs.Empty);
        }
        public ObservableCollection<TantargyatlagViewModel> Tantargyak { get; set; }
        public void SetUpTantargyak()
        {
            Tantargyak.Clear();

            List<GradeListDto> grades = model.GetGrades();

            if (grades.Count == 0)
                return;

            string prname = grades[0].tantargyNev;
            List<GradeListDto> nowgrades = new List<GradeListDto>();

            foreach (GradeListDto item in grades)
            {
                if (item.tantargyNev == prname)
                {
                    nowgrades.Add(item);
                }
                else
                {
                    Tantargyak.Add(new TantargyatlagViewModel(prname, nowgrades));
                    prname = item.tantargyNev;
                    nowgrades = new List<GradeListDto>(); // FIX 1: új lista, ne Clear()
                    nowgrades.Add(item);
                }
            }

            // FIX 2: az utolsó tantárgy csoport hozzáadása
            if (nowgrades.Count > 0)
                Tantargyak.Add(new TantargyatlagViewModel(prname, nowgrades));
        }
    }
}

