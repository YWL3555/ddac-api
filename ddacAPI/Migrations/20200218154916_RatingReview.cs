using Microsoft.EntityFrameworkCore.Migrations;

namespace ddacAPI.Migrations
{
    public partial class RatingReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RatingReview_Hotel_HotelId",
                table: "RatingReview");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "RatingReview",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RatingReview_Hotel_HotelId",
                table: "RatingReview",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RatingReview_Hotel_HotelId",
                table: "RatingReview");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "RatingReview",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_RatingReview_Hotel_HotelId",
                table: "RatingReview",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
