using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DataLayer;

namespace DataLayer.Migrations
{
    [DbContext(typeof(CalDAVSQLiteContext))]
    partial class CalDAVSQLiteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("DataLayer.Models.ACL.Principal", b =>
                {
                    b.Property<int>("PrincipalId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PrincipalStringIdentifier");

                    b.Property<string>("PrincipalURL")
                        .IsRequired();

                    b.Property<string>("SessionId");

                    b.HasKey("PrincipalId");

                    b.HasAlternateKey("PrincipalURL");

                    b.ToTable("Principals");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarCollection", b =>
                {
                    b.Property<int>("CalendarCollectionId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CalendarHomeId");

                    b.Property<string>("Name");

                    b.Property<int?>("PrincipalId");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("CalendarCollectionId");

                    b.HasAlternateKey("Url");

                    b.HasIndex("CalendarHomeId");

                    b.HasIndex("PrincipalId");

                    b.ToTable("CalendarCollections");
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

                    b.HasAlternateKey("Href");

                    b.HasIndex("CalendarCollectionId");

                    b.ToTable("CalendarResources");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.Property", b =>
                {
                    b.Property<int>("PropertyId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CalendarCollectionId");

                    b.Property<int?>("CalendarHomeId");

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

                    b.HasIndex("CalendarCollectionId");

                    b.HasIndex("CalendarHomeId");

                    b.HasIndex("CalendarResourceId");

                    b.HasIndex("PricipalId");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.ResourcesAndCollections.CalendarHome", b =>
                {
                    b.Property<int>("CalendarHomeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("PrincipalId");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("CalendarHomeId");

                    b.HasIndex("PrincipalId")
                        .IsUnique();

                    b.ToTable("CalendarHomeCollections");
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

                    b.HasIndex("PrincipalId")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.Student", b =>
                {
                    b.HasBaseType("DataLayer.Models.Entities.User");

                    b.Property<string>("Career");

                    b.Property<string>("Group");

                    b.Property<int>("Year");

                    b.ToTable("Student");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.Worker", b =>
                {
                    b.HasBaseType("DataLayer.Models.Entities.User");

                    b.Property<string>("Deparment");

                    b.Property<string>("Faculty");

                    b.ToTable("Worker");

                    b.HasDiscriminator().HasValue("Worker");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarCollection", b =>
                {
                    b.HasOne("DataLayer.Models.Entities.ResourcesAndCollections.CalendarHome", "CalendarHome")
                        .WithMany("CalendarCollections")
                        .HasForeignKey("CalendarHomeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLayer.Models.ACL.Principal", "Principal")
                        .WithMany("CalendarCollections")
                        .HasForeignKey("PrincipalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Entities.CalendarResource", b =>
                {
                    b.HasOne("DataLayer.Models.Entities.CalendarCollection", "CalendarCollection")
                        .WithMany("CalendarResources")
                        .HasForeignKey("CalendarCollectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Entities.Property", b =>
                {
                    b.HasOne("DataLayer.Models.Entities.CalendarCollection", "CalendarCollection")
                        .WithMany("Properties")
                        .HasForeignKey("CalendarCollectionId");

                    b.HasOne("DataLayer.Models.Entities.ResourcesAndCollections.CalendarHome", "CalendarHome")
                        .WithMany("Properties")
                        .HasForeignKey("CalendarHomeId");

                    b.HasOne("DataLayer.Models.Entities.CalendarResource", "CalendarResource")
                        .WithMany("Properties")
                        .HasForeignKey("CalendarResourceId");

                    b.HasOne("DataLayer.Models.ACL.Principal", "Principal")
                        .WithMany("Properties")
                        .HasForeignKey("PricipalId");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.ResourcesAndCollections.CalendarHome", b =>
                {
                    b.HasOne("DataLayer.Models.ACL.Principal", "Principal")
                        .WithOne("CalendarHome")
                        .HasForeignKey("DataLayer.Models.Entities.ResourcesAndCollections.CalendarHome", "PrincipalId");
                });

            modelBuilder.Entity("DataLayer.Models.Entities.User", b =>
                {
                    b.HasOne("DataLayer.Models.ACL.Principal", "Principal")
                        .WithOne("User")
                        .HasForeignKey("DataLayer.Models.Entities.User", "PrincipalId");
                });
        }
    }
}
