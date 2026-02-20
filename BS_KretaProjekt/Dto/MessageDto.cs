namespace BS_KretaProjekt.Dto
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string tartalom { get; set; }
        public string cim { get; set; }

        public string kuldoname { get; set; }/*küldő user ->admin vagy tanar*/

        public DateTimeOffset kuldesidopontja { get; set; }
    }
}
