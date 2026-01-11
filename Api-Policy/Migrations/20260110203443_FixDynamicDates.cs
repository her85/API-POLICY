using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Policy.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2027, 1, 10, 17, 31, 10, 223, DateTimeKind.Local).AddTicks(6993), new DateTime(2026, 1, 10, 17, 31, 10, 222, DateTimeKind.Local).AddTicks(7214) });
        }
    }
}
