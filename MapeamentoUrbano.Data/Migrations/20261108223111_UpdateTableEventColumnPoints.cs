using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapeamentoUrbano.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableEventColumnPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Event",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Event");
        }
    }
}
