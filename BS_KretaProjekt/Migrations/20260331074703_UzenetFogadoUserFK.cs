using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BS_KretaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class UzenetFogadoUserFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uzenetek_Diakok_fogado_id",
                table: "Uzenetek");

            migrationBuilder.DropIndex(
                name: "IX_Diakok_emailcim",
                table: "Diakok");

            migrationBuilder.AddForeignKey(
                name: "FK_Uzenetek_Users_fogado_id",
                table: "Uzenetek",
                column: "fogado_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uzenetek_Users_fogado_id",
                table: "Uzenetek");

            migrationBuilder.CreateIndex(
                name: "IX_Diakok_emailcim",
                table: "Diakok",
                column: "emailcim",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Uzenetek_Diakok_fogado_id",
                table: "Uzenetek",
                column: "fogado_id",
                principalTable: "Diakok",
                principalColumn: "diak_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
