using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntraNet.Migrations
{
    /// <inheritdoc />
    public partial class AddAutorIdToProcesso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Processos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutorId",
                table: "Processos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AutorId",
                table: "Avisos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avisos_AutorId",
                table: "Avisos",
                column: "AutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avisos_AspNetUsers_AutorId",
                table: "Avisos",
                column: "AutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avisos_AspNetUsers_AutorId",
                table: "Avisos");

            migrationBuilder.DropIndex(
                name: "IX_Avisos_AutorId",
                table: "Avisos");

            migrationBuilder.DropColumn(
                name: "AutorId",
                table: "Processos");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Processos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AutorId",
                table: "Avisos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
