using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMoney.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCanManageCardsFromCardUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "can_manage_cards",
                table: "card_user_profiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "can_manage_cards",
                table: "card_user_profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
