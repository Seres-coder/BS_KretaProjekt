using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BS_KretaProjekt.Persistence
{
    public class KretaDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Tanar> Tanarok { get; set; }
        public DbSet<Diak> Diakok { get; set; }
        public DbSet<Tantargy> Tantargyok { get; set; }
        public DbSet<Uzenet> Uzenetek { get; set; }
        public DbSet<Jegy> Jegyek { get; set; }
        public DbSet<Orarend> Orarendek { get; set; }
        public DbSet<Osztaly> Osztalyok { get; set; }
        public DbSet<Hianyzas> Hianyzasok { get; set; }


        public KretaDbContext(DbContextOptions<KretaDbContext> options) : base(options) { }
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public string belepesnev { get; set; }
        public string jelszo { get; set; }
        public string Role { get; set; } = "User";
    }
    public class Tanar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tanar_id { get; set; }
        public string tanar_nev { get; set; }
        public string szak { get; set; }
        public int diak_id { get; set; }
        public List<Diak> diak { get; set; }

        public int jegy_id { get; set; }
        public List<Jegy> jegy { get; set; }
        public int tantargy_id { get; set; }
        public Tantargy Tantargy { get; set; }

        public string Role { get; set; } = "Tanar";
        public int user_id { get; set; }
        public User User { get; set; }
    }

    public class Diak
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int diak_id { get; set; }

        public string diak_nev { get; set; }
        public int user_id { get; set; }
        public User User { get; set; }

        public int osztaly_id { get; set; }
        public Osztaly Osztaly { get; set; }

        public int tanar_id { get; set; }
        public List<Tanar> Tanar { get; set; }
        public DateTime szuletesi_datum { get; set; }
        public string lakcim { get; set; }
        public string szuloneve { get; set; }
        public string emailcim { get; set; }
        public int jegyek { get; set; }

        public string Role { get; set; } = "Diak";
    }
    public class Tantargy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tantargy_id { get; set; }
        public string tantargy_nev { get; set; }
    }

    public class Uzenet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int uzenet_id { get; set; }
        public string tartalom { get; set; }
        public string cim { get; set; }

        public int fogado_id { get; set; }/*fogado usAer ->diak*/
        public Diak Fogado { get; set; }
        public int user_id { get; set; }/*küldő user ->admin vagy tanar*/
        public User User { get; set; }
        public DateTimeOffset kuldesidopontja { get; set; }

    }

    public class Jegy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int jegy_id { get; set; }
        public DateTimeOffset datum { get; set; }
        public DateTimeOffset updatedatum { get; set; }
        public int ertek { get; set; }

        public int tantargy_id { get; set; }
        public Tantargy tantargy { get; set; }

        public int tanar_id { get; set; }
        public List<Tanar> Tanar { get; set; }

        public int diak_id { get; set; }
        public List<Diak> diak { get; set; }
    }

    public class Orarend
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orarend_id { get; set; }
        public int osztaly_id { get; set; }
        public Osztaly osztaly { get; set; }

        public DayOfWeek nap { get; set; }
        public int ora { get; set; }  // hányadik óra 0-9
        public int tantargy_id { get; set; }
        public Tantargy tantargy { get; set; }
        public int tanar_id { get; set; }
        public Tanar Tanar { get; set; }
    }

    public class Osztaly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int osztaly_id { get; set; }
        public string osztaly_nev { get; set; }
    }
    public class Hianyzas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hianyzas_id { get; set; }
        public int hianyzottorakszama { get; set; }

    }

}

