using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Index(nameof(belepesnev), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }

        [Required]
        public string belepesnev { get; set; } = string.Empty;

        [Required]
        public string jelszo { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";
    }

    public class Tanar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tanar_id { get; set; }

        public string? tanar_nev { get; set; }
        public string? szak { get; set; }

        public int? tantargy_id { get; set; }

        [ForeignKey(nameof(tantargy_id))]
        public Tantargy? Tantargy { get; set; }

        [Required]
        public int user_id { get; set; }

        [ForeignKey(nameof(user_id))]
        public User? User { get; set; }

        public List<Jegy>? jegyek { get; set; }
        public List<Orarend>? Orarend { get; set; }
    }

    [Index(nameof(emailcim), IsUnique = true)]
    public class Diak
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int diak_id { get; set; }

        public string? diak_nev { get; set; }

        [Required]
        public int user_id { get; set; }

        [ForeignKey(nameof(user_id))]
        public User? User { get; set; }

        public int? osztaly_id { get; set; }

        [ForeignKey(nameof(osztaly_id))]
        public Osztaly? Osztaly { get; set; }

        public DateTime? szuletesi_datum { get; set; }
        public string? lakcim { get; set; }
        public string? szuloneve { get; set; }
        public string? emailcim { get; set; }

        public List<Jegy>? jegyek { get; set; }
    }

    [Index(nameof(tantargy_nev), IsUnique = true)]
    public class Tantargy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tantargy_id { get; set; }

        [Required]
        public string tantargy_nev { get; set; } = string.Empty;

        public List<Orarend>? Orarend { get; set; }
    }

    public class Uzenet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int uzenet_id { get; set; }

        [Required]
        public string tartalom { get; set; } = string.Empty;

        [Required]
        public string cim { get; set; } = string.Empty;

        public int fogado_id { get; set; }

        [ForeignKey(nameof(fogado_id))]
        public Diak? Fogado { get; set; }

        public int user_id { get; set; }

        [ForeignKey(nameof(user_id))]
        public User? User { get; set; }

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

        [ForeignKey(nameof(tantargy_id))]
        public Tantargy? Tantargy { get; set; }

        public int tanar_id { get; set; }

        [ForeignKey(nameof(tanar_id))]
        public Tanar? Tanar { get; set; }

        public int diak_id { get; set; }

        [ForeignKey(nameof(diak_id))]
        public Diak? Diak { get; set; }
    }

    public class Orarend
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orarend_id { get; set; }

        public int osztaly_id { get; set; }

        [ForeignKey(nameof(osztaly_id))]
        public Osztaly? Osztaly { get; set; }

        public DayOfWeek nap { get; set; }
        public int ora { get; set; }

        public int tantargy_id { get; set; }

        [ForeignKey(nameof(tantargy_id))]
        public Tantargy? Tantargy { get; set; }

        public int tanar_id { get; set; }

        [ForeignKey(nameof(tanar_id))]
        public Tanar? Tanar { get; set; }
    }

    [Index(nameof(osztaly_nev), IsUnique = true)]
    public class Osztaly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int osztaly_id { get; set; }

        [Required]
        public string osztaly_nev { get; set; } = string.Empty;

        public List<Orarend> Orarend { get; set; } = new();
    }

    public class Hianyzas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hianyzas_id { get; set; }

        public int hianyzottorakszama { get; set; }
    }
}