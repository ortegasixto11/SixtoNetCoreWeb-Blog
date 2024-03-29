﻿// <auto-generated />
using System;
using HandKrossBlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HandKrossBlog.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190913163513_ModelAnswer")]
    partial class ModelAnswer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HandKrossBlog.Models.Answer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<Guid>("QuestionId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("HandKrossBlog.Models.BlogComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BlogPostId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Text");

                    b.Property<string>("User");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("HandKrossBlog.Models.BlogPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("HandKrossBlog.Models.BlogVisit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("IpAddress");

                    b.Property<Guid>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("HandKrossBlog.Models.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("HandKrossBlog.Models.Answer", b =>
                {
                    b.HasOne("HandKrossBlog.Models.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HandKrossBlog.Models.BlogComment", b =>
                {
                    b.HasOne("HandKrossBlog.Models.BlogPost", "BlogPost")
                        .WithMany("BlogPostComments")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HandKrossBlog.Models.BlogVisit", b =>
                {
                    b.HasOne("HandKrossBlog.Models.BlogPost", "Post")
                        .WithMany("BlogPostVisits")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
