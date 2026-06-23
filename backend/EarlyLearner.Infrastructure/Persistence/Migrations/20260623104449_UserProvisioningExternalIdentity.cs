using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyLearner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserProvisioningExternalIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "external_tenant_id",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "external_object_id",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_users_external_object_id_external_tenant_id",
                table: "users",
                columns: new[] { "external_object_id", "external_tenant_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_external_object_id_external_tenant_id",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "external_tenant_id",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "external_object_id",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
