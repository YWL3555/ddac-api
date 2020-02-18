using Microsoft.EntityFrameworkCore.Migrations;

namespace ddacAPI.Migrations
{
    public partial class Booking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_RoomType_RoomTypeId",
                table: "Booking");

            migrationBuilder.AlterColumn<int>(
                name: "RoomTypeId",
                table: "Booking",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_RoomType_RoomTypeId",
                table: "Booking",
                column: "RoomTypeId",
                principalTable: "RoomType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_RoomType_RoomTypeId",
                table: "Booking");

            migrationBuilder.AlterColumn<int>(
                name: "RoomTypeId",
                table: "Booking",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_RoomType_RoomTypeId",
                table: "Booking",
                column: "RoomTypeId",
                principalTable: "RoomType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
