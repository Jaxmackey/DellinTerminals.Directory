using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DellinTerminals.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_phones_offices_office_id",
                table: "phones");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "offices");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "offices");

            migrationBuilder.RenameColumn(
                name: "office_id",
                table: "phones",
                newName: "OfficeId");

            migrationBuilder.RenameIndex(
                name: "ix_phones_office_id",
                table: "phones",
                newName: "IX_phones_OfficeId");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "offices",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_phones_offices_OfficeId",
                table: "phones",
                column: "OfficeId",
                principalTable: "offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_phones_offices_OfficeId",
                table: "phones");

            migrationBuilder.RenameColumn(
                name: "OfficeId",
                table: "phones",
                newName: "office_id");

            migrationBuilder.RenameIndex(
                name: "IX_phones_OfficeId",
                table: "phones",
                newName: "ix_phones_office_id");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "offices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "offices",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "offices",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_phones_offices_office_id",
                table: "phones",
                column: "office_id",
                principalTable: "offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
