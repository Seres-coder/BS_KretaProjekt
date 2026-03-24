using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BS_KretaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hianyzasok",
                columns: table => new
                {
                    hianyzas_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hianyzottorakszama = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hianyzasok", x => x.hianyzas_id);
                });

            migrationBuilder.CreateTable(
                name: "Osztalyok",
                columns: table => new
                {
                    osztaly_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    osztaly_nev = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Osztalyok", x => x.osztaly_id);
                });

            migrationBuilder.CreateTable(
                name: "Tantargyok",
                columns: table => new
                {
                    tantargy_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tantargy_nev = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tantargyok", x => x.tantargy_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    belepesnev = table.Column<string>(type: "text", nullable: false),
                    jelszo = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Diakok",
                columns: table => new
                {
                    diak_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    diak_nev = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    osztaly_id = table.Column<int>(type: "integer", nullable: true),
                    szuletesi_datum = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lakcim = table.Column<string>(type: "text", nullable: true),
                    szuloneve = table.Column<string>(type: "text", nullable: true),
                    emailcim = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diakok", x => x.diak_id);
                    table.ForeignKey(
                        name: "FK_Diakok_Osztalyok_osztaly_id",
                        column: x => x.osztaly_id,
                        principalTable: "Osztalyok",
                        principalColumn: "osztaly_id");
                    table.ForeignKey(
                        name: "FK_Diakok_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tanarok",
                columns: table => new
                {
                    tanar_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tanar_nev = table.Column<string>(type: "text", nullable: true),
                    szak = table.Column<string>(type: "text", nullable: true),
                    tantargy_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanarok", x => x.tanar_id);
                    table.ForeignKey(
                        name: "FK_Tanarok_Tantargyok_tantargy_id",
                        column: x => x.tantargy_id,
                        principalTable: "Tantargyok",
                        principalColumn: "tantargy_id");
                    table.ForeignKey(
                        name: "FK_Tanarok_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Uzenetek",
                columns: table => new
                {
                    uzenet_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tartalom = table.Column<string>(type: "text", nullable: false),
                    cim = table.Column<string>(type: "text", nullable: false),
                    fogado_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    kuldesidopontja = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzenetek", x => x.uzenet_id);
                    table.ForeignKey(
                        name: "FK_Uzenetek_Diakok_fogado_id",
                        column: x => x.fogado_id,
                        principalTable: "Diakok",
                        principalColumn: "diak_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Uzenetek_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jegyek",
                columns: table => new
                {
                    jegy_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    datum = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedatum = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ertek = table.Column<int>(type: "integer", nullable: false),
                    tantargy_id = table.Column<int>(type: "integer", nullable: false),
                    tanar_id = table.Column<int>(type: "integer", nullable: false),
                    diak_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jegyek", x => x.jegy_id);
                    table.ForeignKey(
                        name: "FK_Jegyek_Diakok_diak_id",
                        column: x => x.diak_id,
                        principalTable: "Diakok",
                        principalColumn: "diak_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jegyek_Tanarok_tanar_id",
                        column: x => x.tanar_id,
                        principalTable: "Tanarok",
                        principalColumn: "tanar_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jegyek_Tantargyok_tantargy_id",
                        column: x => x.tantargy_id,
                        principalTable: "Tantargyok",
                        principalColumn: "tantargy_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orarendek",
                columns: table => new
                {
                    orarend_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    osztaly_id = table.Column<int>(type: "integer", nullable: false),
                    nap = table.Column<int>(type: "integer", nullable: false),
                    ora = table.Column<int>(type: "integer", nullable: false),
                    tantargy_id = table.Column<int>(type: "integer", nullable: false),
                    tanar_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orarendek", x => x.orarend_id);
                    table.ForeignKey(
                        name: "FK_Orarendek_Osztalyok_osztaly_id",
                        column: x => x.osztaly_id,
                        principalTable: "Osztalyok",
                        principalColumn: "osztaly_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orarendek_Tanarok_tanar_id",
                        column: x => x.tanar_id,
                        principalTable: "Tanarok",
                        principalColumn: "tanar_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orarendek_Tantargyok_tantargy_id",
                        column: x => x.tantargy_id,
                        principalTable: "Tantargyok",
                        principalColumn: "tantargy_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diakok_emailcim",
                table: "Diakok",
                column: "emailcim",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diakok_osztaly_id",
                table: "Diakok",
                column: "osztaly_id");

            migrationBuilder.CreateIndex(
                name: "IX_Diakok_user_id",
                table: "Diakok",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Jegyek_diak_id",
                table: "Jegyek",
                column: "diak_id");

            migrationBuilder.CreateIndex(
                name: "IX_Jegyek_tanar_id",
                table: "Jegyek",
                column: "tanar_id");

            migrationBuilder.CreateIndex(
                name: "IX_Jegyek_tantargy_id",
                table: "Jegyek",
                column: "tantargy_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orarendek_osztaly_id",
                table: "Orarendek",
                column: "osztaly_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orarendek_tanar_id",
                table: "Orarendek",
                column: "tanar_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orarendek_tantargy_id",
                table: "Orarendek",
                column: "tantargy_id");

            migrationBuilder.CreateIndex(
                name: "IX_Osztalyok_osztaly_nev",
                table: "Osztalyok",
                column: "osztaly_nev",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tanarok_tantargy_id",
                table: "Tanarok",
                column: "tantargy_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tanarok_user_id",
                table: "Tanarok",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tantargyok_tantargy_nev",
                table: "Tantargyok",
                column: "tantargy_nev",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_belepesnev",
                table: "Users",
                column: "belepesnev",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uzenetek_fogado_id",
                table: "Uzenetek",
                column: "fogado_id");

            migrationBuilder.CreateIndex(
                name: "IX_Uzenetek_user_id",
                table: "Uzenetek",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hianyzasok");

            migrationBuilder.DropTable(
                name: "Jegyek");

            migrationBuilder.DropTable(
                name: "Orarendek");

            migrationBuilder.DropTable(
                name: "Uzenetek");

            migrationBuilder.DropTable(
                name: "Tanarok");

            migrationBuilder.DropTable(
                name: "Diakok");

            migrationBuilder.DropTable(
                name: "Tantargyok");

            migrationBuilder.DropTable(
                name: "Osztalyok");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
