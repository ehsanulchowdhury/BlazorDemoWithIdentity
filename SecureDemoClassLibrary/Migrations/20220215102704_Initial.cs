using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureDemoClassLibrary.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13f8f79a-1c1d-4aa1-afb0-6570304b9cf8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "115dac8e-e920-4687-b4fa-e470bf29d515");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "158688a4-00ea-4525-b103-876af278095f", "8659c654-890c-47d1-81e8-5b770409cf74", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserEntityId", "UserEntityType", "UserName" },
                values: new object[] { "1740b84d-d3c6-48ad-b338-f7e1caf20e89", 0, "2be883ab-42fd-4b65-bbed-a92f9c3a97f5", "admin@gmail.com", false, "Admin", false, "Admin", false, null, null, null, "AQAAAAEAACcQAAAAEBJ8FaNpIEcNUweYppxPH9Oc0yo7jsn/RMG2Avry7d6kTP35tVKSrJHj2xGNIUuHmw==", "1234567890", false, "c5703c87-b1b9-4c1a-b196-d71690c814b6", false, 0, "Admin", "admin@gmail.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "158688a4-00ea-4525-b103-876af278095f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1740b84d-d3c6-48ad-b338-f7e1caf20e89");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "13f8f79a-1c1d-4aa1-afb0-6570304b9cf8", "8d634281-3532-40ef-9efe-bc71f6aaee18", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserEntityId", "UserEntityType", "UserName" },
                values: new object[] { "115dac8e-e920-4687-b4fa-e470bf29d515", 0, "b7d8788e-91ca-4e18-bdd6-b6d33b4cf6ae", "admin@gmail.com", false, "Admin", false, "Admin", false, null, null, null, "AQAAAAEAACcQAAAAEKbJmerId/zSRuW+1ik0l0O2P2kc8qhQErKt0rJvoUvj0irSowUxa9+sgNqN+jboFA==", "1234567890", false, "e0f39dc6-738b-4aa0-b71b-9fff717f6c1c", false, 0, "Admin", "admin@gmail.com" });
        }
    }
}
