using Microsoft.EntityFrameworkCore.Migrations;

namespace HandKrossBlog.Migrations
{
    public partial class Answer_PositivePoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositivePoints",
                table: "Answers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositivePoints",
                table: "Answers");
        }
    }
}
