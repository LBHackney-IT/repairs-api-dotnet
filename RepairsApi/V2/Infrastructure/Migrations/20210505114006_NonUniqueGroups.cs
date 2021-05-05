using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class NonUniqueGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_security_groups",
                table: "security_groups");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "security_groups",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_security_groups",
                table: "security_groups",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_security_groups_group_name",
                table: "security_groups",
                column: "group_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_security_groups",
                table: "security_groups");

            migrationBuilder.DropIndex(
                name: "ix_security_groups_group_name",
                table: "security_groups");

            migrationBuilder.DropColumn(
                name: "id",
                table: "security_groups");

            migrationBuilder.AddPrimaryKey(
                name: "pk_security_groups",
                table: "security_groups",
                column: "group_name");
        }
    }
}
