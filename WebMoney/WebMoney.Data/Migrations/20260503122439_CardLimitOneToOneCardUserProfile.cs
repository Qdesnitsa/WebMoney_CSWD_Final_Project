using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMoney.Data.Migrations
{
    /// <inheritdoc />
    public partial class CardLimitOneToOneCardUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_card_user_profiles_card_limit_id",
                table: "card_user_profiles");

            migrationBuilder.CreateIndex(
                name: "ix_card_user_profiles_card_limit_id",
                table: "card_user_profiles",
                column: "card_limit_id",
                unique: true,
                filter: "card_limit_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_card_user_profiles_card_limit_id",
                table: "card_user_profiles");

            migrationBuilder.CreateIndex(
                name: "ix_card_user_profiles_card_limit_id",
                table: "card_user_profiles",
                column: "card_limit_id");
        }
    }
}
