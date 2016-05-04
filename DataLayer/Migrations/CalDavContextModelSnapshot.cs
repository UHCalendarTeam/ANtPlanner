using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;

namespace DataLayer.Migrations
{
    [DbContext(typeof (CalDavContext))]
    internal class CalDavContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataLayer.Models.ACL.Principal", b =>
            {
                b.Property<int>("PrincipalId")
                    .ValueGeneratedOnAdd();

                b.Property<string>("PrincipalStringIdentifier");

                b.Property<string>("PrincipalURL");

                b.Property<string>("SessionId");

                b.HasKey("PrincipalId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarCollection", b =>
            {
                b.Property<int>("CalendarCollectionId")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Name");

                b.Property<int?>("PrincipalId");

                b.Property<string>("Url")
                    .IsRequired();

                b.HasKey("CalendarCollectionId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarResource", b =>
            {
                b.Property<int>("CalendarResourceId")
                    .ValueGeneratedOnAdd();

                b.Property<int>("CalendarCollectionId");

                b.Property<string>("Href")
                    .IsRequired();

                b.Property<string>("Name")
                    .IsRequired();

                b.Property<string>("Uid");

                b.HasKey("CalendarResourceId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.Property", b =>
            {
                b.Property<int>("PropertyId")
                    .ValueGeneratedOnAdd();

                b.Property<int?>("CalendarCollectionId");

                b.Property<int?>("CalendarResourceId");

                b.Property<bool>("IsDestroyable");

                b.Property<bool>("IsMutable");

                b.Property<bool>("IsVisible");

                b.Property<string>("Name")
                    .IsRequired();

                b.Property<string>("Namespace")
                    .IsRequired();

                b.Property<int?>("PricipalId");

                b.Property<string>("Value");

                b.HasKey("PropertyId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.User", b =>
            {
                b.Property<int>("UserId")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Discriminator")
                    .IsRequired();

                b.Property<string>("DisplayName");

                b.Property<string>("Email")
                    .IsRequired();

                b.Property<string>("Password");

                b.Property<int?>("PrincipalId");

                b.HasKey("UserId");

                b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                b.HasAnnotation("Relational:DiscriminatorValue", "User");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.Student", b =>
            {
                b.HasBaseType("DataLayer.Models.Entities.User");

                b.Property<string>("Career");

                b.Property<string>("Group");

                b.Property<int>("Year");

                b.HasAnnotation("Relational:DiscriminatorValue", "Student");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.Worker", b =>
            {
                b.HasBaseType("DataLayer.Models.Entities.User");

                b.Property<string>("Deparment");

                b.Property<string>("Faculty");

                b.HasAnnotation("Relational:DiscriminatorValue", "Worker");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarCollection", b =>
            {
                b.HasOne("DataLayer.Models.ACL.Principal")
                    .WithMany()
                    .HasForeignKey("PrincipalId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarResource", b =>
            {
                b.HasOne("DataLayer.Models.Entities.CalendarCollection")
                    .WithMany()
                    .HasForeignKey("CalendarCollectionId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.Property", b =>
            {
                b.HasOne("DataLayer.Models.Entities.CalendarCollection")
                    .WithMany()
                    .HasForeignKey("CalendarCollectionId");

                b.HasOne("DataLayer.Models.Entities.CalendarResource")
                    .WithMany()
                    .HasForeignKey("CalendarResourceId");

                b.HasOne("DataLayer.Models.ACL.Principal")
                    .WithMany()
                    .HasForeignKey("PricipalId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.User", b =>
            {
                b.HasOne("DataLayer.Models.ACL.Principal")
                    .WithOne()
                    .HasForeignKey("DataLayer.Models.Entities.User", "PrincipalId");
            });

            modelBuilder.Entity("DataLayer.Models.Entities.Student", b => { });

            modelBuilder.Entity("DataLayer.Models.Entities.Worker", b => { });
        }
    }
}