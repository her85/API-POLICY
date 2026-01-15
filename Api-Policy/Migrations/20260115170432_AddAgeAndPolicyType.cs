using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Policy.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeAndPolicyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientAge",
                table: "Policies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Policies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClientAge", "Type" },
                values: new object[] { 0, 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientAge",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Policies");
        }
    }
}
