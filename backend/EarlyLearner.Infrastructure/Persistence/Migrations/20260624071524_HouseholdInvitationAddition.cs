using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyLearner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HouseholdInvitationAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "given_name",
                table: "children",
                newName: "first_name");

            migrationBuilder.RenameIndex(
                name: "ix_children_household_id_given_name",
                table: "children",
                newName: "ix_children_household_id_first_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "children",
                newName: "given_name");

            migrationBuilder.RenameIndex(
                name: "ix_children_household_id_first_name",
                table: "children",
                newName: "ix_children_household_id_given_name");
        }
    }
}
