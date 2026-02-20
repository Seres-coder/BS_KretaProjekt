namespace BS_KretaProjekt.Dto
{
    public class UpdateOrarendDto
    {
        public int orarend_id { get; set; }
        public string osztaly_nev { get; set; }
        public DayOfWeek nap { get; set; }
        public int ora { get; set; }
        public string tantargy_nev { get; set; }
        public string tanar_nev { get; set; }
    }
}
