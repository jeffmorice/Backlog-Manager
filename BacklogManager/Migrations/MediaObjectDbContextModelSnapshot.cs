﻿// <auto-generated />
using System;
using BacklogManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BacklogManager.Migrations
{
    [DbContext(typeof(MediaObjectDbContext))]
    partial class MediaObjectDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BacklogManager.Models.MediaObject", b =>
            {
                b.Property<int>("ID")
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<bool>("Completed");

                b.Property<int>("DatabaseSource");

                b.Property<DateTime>("DateAdded")
                    .HasColumnType("date");

                b.Property<bool>("Deleted");

                b.Property<string>("OwnerId");

                b.Property<string>("RecommendSource");

                b.Property<bool>("Started");

                b.Property<int>("SubTypeID");

                b.Property<string>("Title")
                    .IsRequired();

                b.Property<int>("UpdateCount");

                b.HasKey("ID");

                b.HasIndex("SubTypeID");

                b.ToTable("MediaObjects");
            });

            modelBuilder.Entity("BacklogManager.Models.SubType", b =>
            {
                b.Property<int>("ID")
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("Name");

                b.Property<int>("ParentID");

                b.HasKey("ID");

                b.ToTable("SubTypes");
            });

            modelBuilder.Entity("BacklogManager.Models.MediaObject", b =>
            {
                b.HasOne("BacklogManager.Models.SubType", "MediaSubType")
                    .WithMany()
                    .HasForeignKey("SubTypeID")
                    .OnDelete(DeleteBehavior.Cascade);
            });
#pragma warning restore 612, 618
        }
    }
}