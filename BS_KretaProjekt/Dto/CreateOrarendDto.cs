namespace BS_KretaProjekt.Dto
{
    public class CreateOrarendDto
    {
        public int osztaly_id { get; set; }
        public DayOfWeek nap { get; set; }
        public int ora { get; set; }
        public string tantargy { get; set; }
        public string Tanarnev { get; set; }
    }
}
