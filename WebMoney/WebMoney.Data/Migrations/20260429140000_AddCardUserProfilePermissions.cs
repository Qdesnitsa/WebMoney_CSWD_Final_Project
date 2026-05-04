using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebMoney.Data;

#nullable disable

namespace WebMoney.Data.Migrations;

[DbContext(typeof(WebContext))]
[Migration("20260429140000_AddCardUserProfilePermissions")]
public partial class AddCardUserProfilePermissions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "can_manage_users",
            table: "card_user_profiles",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "can_manage_cards",
            table: "card_user_profiles",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.Sql(
            """
            UPDATE card_user_profiles AS cup
            SET can_manage_users = true, can_manage_cards = true
            FROM cards AS c, users AS u
            WHERE cup.card_id = c.id
              AND u.id = cup.user_id
              AND lower(u.email) = lower(c.created_by);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "can_manage_users",
            table: "card_user_profiles");

        migrationBuilder.DropColumn(
            name: "can_manage_cards",
            table: "card_user_profiles");
    }
}
