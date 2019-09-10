using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HandKrossBlog.Migrations
{
    public partial class VisitPostId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Posts_PostId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "PropId",
                table: "Visits");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "Visits",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Posts_PostId",
                table: "Visits",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Posts_PostId",
                table: "Visits");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "Visits",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "PropId",
                table: "Visits",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Posts_PostId",
                table: "Visits",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
