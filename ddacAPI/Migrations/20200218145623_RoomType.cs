using Microsoft.EntityFrameworkCore.Migrations;

namespace ddacAPI.Migrations
{
    public partial class RoomType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomType_Hotel_HotelId",
                table: "RoomType");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "RoomType",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomType_Hotel_HotelId",
                table: "RoomType",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomType_Hotel_HotelId",
                table: "RoomType");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "RoomType",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_RoomType_Hotel_HotelId",
                table: "RoomType",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
