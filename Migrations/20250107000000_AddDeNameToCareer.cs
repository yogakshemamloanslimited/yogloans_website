using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yogloansdotnet.Migrations
{
    /// <inheritdoc />
    public partial class AddDeNameToCareer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "de_name",
                table: "Career",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "de_name",
                table: "Career");
        }
    }
} 