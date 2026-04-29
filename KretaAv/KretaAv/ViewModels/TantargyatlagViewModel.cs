using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KretaAv.Dto;

namespace KretaAv.ViewModels
{
    public class TantargyatlagViewModel
    {
        private string _name;
        private List<GradeListDto> jegyek;


        public string Name { get { return _name; } }
        public double Avg { get { return avgcounter(); } }
        private double avgcounter()
        {
            double sum = 0;

            foreach (GradeListDto item in jegyek)
            {
                sum += item.ertek;
            }
            return sum / jegyek.Count;
        }
        public TantargyatlagViewModel(string name, List<GradeListDto> dto)
        {
            _name = name;
            jegyek = dto;
        }
    }
}
