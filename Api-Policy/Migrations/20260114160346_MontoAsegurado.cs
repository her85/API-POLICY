using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Policy.Migrations
{
    /// <inheritdoc />
    public partial class MontoAsegurado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClientName", "PolicyNumber" },
                values: new object[] { "Juan Ramírez", "POL-F8B6B647" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClientName", "PolicyNumber" },
                values: new object[] { "Juan B", "POL-2024-001" });
        }
    }
}
