using Microsoft.EntityFrameworkCore.Migrations;

namespace Metomarket.Data.Migrations
{
    public partial class FixCreditCompanyNameMaxLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CreditCompanies",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CreditCompanies",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 25);
        }
    }
}