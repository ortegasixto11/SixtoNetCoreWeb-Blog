using Microsoft.EntityFrameworkCore.Migrations;

namespace HandKrossBlog.Migrations
{
    public partial class AgregandoCamposUserHandkross : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Posts",
                newName: "Specialty_customized");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "Specialty_customized",
                table: "Posts",
                newName: "CreatedBy");
        }
    }
}
