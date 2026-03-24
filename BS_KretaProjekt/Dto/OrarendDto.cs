namespace BS_KretaProjekt.Dto
{
    public class OrarendDto
    {
        public int Id { get; set; }
        public string osztalyNev { get; set; }
        public DayOfWeek nap { get; set; }
        public string ora { get; set; }
        public string tantargyNev { get; set; }
        public string tanarNev
        {
            get; set;
        }
    }
}

