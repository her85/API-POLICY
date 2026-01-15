using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Policy.Migrations
{
    /// <inheritdoc />
    public partial class PolicyNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClientAge", "Type" },
                values: new object[] { 35, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClientAge", "Type" },
                values: new object[] { 0, 4 });
        }
    }
}
