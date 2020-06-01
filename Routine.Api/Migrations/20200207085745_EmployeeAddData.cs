using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class EmployeeAddData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("078563f3-37ec-6d6f-edf1-31ddc857db19"), new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G003", "Mary", 2, "King" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("7692dcdc-19e4-1e66-c6ae-2de54a696b25"), new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), new DateTime(1996, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "G097", "Kevin", 1, "Richardson" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("0f3e84ac-b19d-ff22-f695-f31dbe3e972a"), new Guid("6fb600c1-9011-4fd7-9234-881379716440"), new DateTime(1976, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSFT231", "NICK", 1, "Carter" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("268e2705-6a3e-52cf-3755-d193cbeb0594"), new Guid("6fb600c1-9011-4fd7-9234-881379716440"), new DateTime(1996, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSFT245", "Vince", 1, "Chart" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("00c66aaf-5f2c-3f49-946f-15c1ad2ea0d3"), new Guid("5efc910b-2f45-43df-afae-620d40542853"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSFT246", "Mary", 2, "Harot" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("e10adc39-49ba-59ab-be56-e057f20f883e"), new Guid("5efc910b-2f45-43df-afae-620d40542853"), new DateTime(1955, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSFT247", "Steve", 1, "Jobs" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("00c66aaf-5f2c-3f49-946f-15c1ad2ea0d3"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("078563f3-37ec-6d6f-edf1-31ddc857db19"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0f3e84ac-b19d-ff22-f695-f31dbe3e972a"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("268e2705-6a3e-52cf-3755-d193cbeb0594"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("7692dcdc-19e4-1e66-c6ae-2de54a696b25"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("e10adc39-49ba-59ab-be56-e057f20f883e"));
        }
    }
}
