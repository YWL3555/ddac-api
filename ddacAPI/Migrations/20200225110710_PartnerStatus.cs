using Microsoft.EntityFrameworkCore.Migrations;

namespace ddacAPI.Migrations
{
    public partial class PartnerStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Partner",
                newName: "partnerStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "partnerStatus",
                table: "Partner",
                newName: "status");
        }
    }
}
