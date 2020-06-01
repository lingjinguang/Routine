using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Routine.Api.Migrations
{
    public partial class AgainInitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Employees");
            migrationBuilder.DropTable("Companies");
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Introduction = table.Column<string>(maxLength: 500, nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: false),
                    Industry = table.Column<string>(maxLength: 50, nullable: false),
                    Product = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    EmployeeNo = table.Column<string>(maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Introduction", "Name", "Country", "Industry", "Product" },
                values: new object[] { new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), "Great Company", "Microsoft", "USA", "Soft", "Soft" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Introduction", "Name", "Country", "Industry", "Product" },
                values: new object[] { new Guid("6fb600c1-9011-4fd7-9234-881379716440"), "Don't be evil", "Google", "USA", "Internet", "Soft" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Introduction", "Name", "Country", "Industry", "Product" },
                values: new object[] { new Guid("5efc910b-2f45-43df-afae-620d40542853"), "Fubao Company", "Alipapa", "ChAIN", "Soft", "Soft" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");


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
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
