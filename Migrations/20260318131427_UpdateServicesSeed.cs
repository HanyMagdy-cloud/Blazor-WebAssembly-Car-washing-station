using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarWashStation.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServicesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Professionell yttre rengöring", "Utvändig tvätt", 400m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Komplett rengöring för både insida och utsida", "In & utvändig tvätt", 750m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Djupgående rengöring och polering", "Rekond", 2500m });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 4, "Kombination av rengöring och skyddande vaxlager", "Tvätt + vax", 1750m },
                    { 5, "Rengöring av motorrummet", "Motortvätt", 350m },
                    { 6, "Grundlig dammsugning och avtorkning", "Invändig tvätt", 400m },
                    { 7, "Bekväm säsongsförvaring inklusive skifte", "Däckförvaring + skifte", 950m },
                    { 8, "Snabb och säker byte av hjul", "Däckskifte", 350m },
                    { 9, "Långvarigt skydd mot repor och UV-ljus", "Lackförsegling", 2295m },
                    { 10, "Professionell reparation av däckskador", "Laga punktering", 350m },
                    { 11, "Montering på fälg och balansering", "Däckmontering", 300m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Exterior wash and dry", "Basic Wash", 15.00m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Exterior wash, interior vacuum, and windows", "Premium Wash", 30.00m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Complete exterior and interior deep cleaning", "Full Detail", 100.00m });
        }
    }
}
