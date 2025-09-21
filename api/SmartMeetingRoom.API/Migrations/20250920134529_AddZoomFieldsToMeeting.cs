using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeetingRoom.API.Migrations
{
    /// <inheritdoc />
    public partial class AddZoomFieldsToMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ZoomJoinUrl",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ZoomMeetingId",
                table: "Meeting",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ZoomJoinUrl",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "ZoomMeetingId",
                table: "Meeting");
        }
    }
}
