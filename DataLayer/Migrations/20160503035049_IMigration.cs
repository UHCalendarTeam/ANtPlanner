using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;

namespace DataLayer.Migrations
{
    public partial class IMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Principal", table => new
            {
                PrincipalId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                PrincipalStringIdentifier = table.Column<string>(nullable: true),
                PrincipalURL = table.Column<string>(nullable: true),
                SessionId = table.Column<string>(nullable: true)
            },
                constraints: table => { table.PrimaryKey("PK_Principal", x => x.PrincipalId); });
            migrationBuilder.CreateTable("CalendarCollection", table => new
            {
                CalendarCollectionId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(nullable: true),
                PrincipalId = table.Column<int>(nullable: true),
                Url = table.Column<string>(nullable: false)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarCollection", x => x.CalendarCollectionId);
                    table.ForeignKey("FK_CalendarCollection_Principal_PrincipalId", x => x.PrincipalId, "Principal",
                        "PrincipalId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("User", table => new
            {
                UserId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                Discriminator = table.Column<string>(nullable: false),
                DisplayName = table.Column<string>(nullable: true),
                Email = table.Column<string>(nullable: false),
                Password = table.Column<string>(nullable: true),
                PrincipalId = table.Column<int>(nullable: true),
                Career = table.Column<string>(nullable: true),
                Group = table.Column<string>(nullable: true),
                Year = table.Column<int>(nullable: true),
                Deparment = table.Column<string>(nullable: true),
                Faculty = table.Column<string>(nullable: true)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey("FK_User_Principal_PrincipalId", x => x.PrincipalId, "Principal", "PrincipalId",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable("CalendarResource", table => new
            {
                CalendarResourceId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                CalendarCollectionId = table.Column<int>(nullable: false),
                Href = table.Column<string>(nullable: false),
                Name = table.Column<string>(nullable: false),
                Uid = table.Column<string>(nullable: true)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarResource", x => x.CalendarResourceId);
                    table.ForeignKey("FK_CalendarResource_CalendarCollection_CalendarCollectionId",
                        x => x.CalendarCollectionId, "CalendarCollection", "CalendarCollectionId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("Property", table => new
            {
                PropertyId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                CalendarCollectionId = table.Column<int>(nullable: true),
                CalendarResourceId = table.Column<int>(nullable: true),
                IsDestroyable = table.Column<bool>(nullable: false),
                IsMutable = table.Column<bool>(nullable: false),
                IsVisible = table.Column<bool>(nullable: false),
                Name = table.Column<string>(nullable: false),
                Namespace = table.Column<string>(nullable: false),
                PricipalId = table.Column<int>(nullable: true),
                Value = table.Column<string>(nullable: true)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyId);
                    table.ForeignKey("FK_Property_CalendarCollection_CalendarCollectionId", x => x.CalendarCollectionId,
                        "CalendarCollection", "CalendarCollectionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Property_CalendarResource_CalendarResourceId", x => x.CalendarResourceId,
                        "CalendarResource", "CalendarResourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Property_Principal_PricipalId", x => x.PricipalId, "Principal", "PrincipalId",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Property");
            migrationBuilder.DropTable("User");
            migrationBuilder.DropTable("CalendarResource");
            migrationBuilder.DropTable("CalendarCollection");
            migrationBuilder.DropTable("Principal");
        }
    }
}