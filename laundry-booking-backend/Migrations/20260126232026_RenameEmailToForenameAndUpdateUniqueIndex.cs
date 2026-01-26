using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace laundry_booking_backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmailToForenameAndUpdateUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Forename");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApartmentNumber",
                table: "Users",
                column: "ApartmentNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ApartmentNumber",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Forename",
                table: "Users",
                newName: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }
    }
}
