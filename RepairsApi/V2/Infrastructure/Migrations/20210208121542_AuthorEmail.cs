using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AuthorEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "author",
                table: "job_status_updates",
                newName: "author_name");

            migrationBuilder.AddColumn<string>(
                name: "author_email",
                table: "job_status_updates",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "author_email",
                table: "job_status_updates");

            migrationBuilder.RenameColumn(
                name: "author_name",
                table: "job_status_updates",
                newName: "author");
        }
    }
}
