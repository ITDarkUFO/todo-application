﻿// <auto-generated />
using System;
using Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240713125321_Set Priority in TodoItem to nullable int")]
    partial class SetPriorityinTodoItemtonullableint
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Application.Models.Priority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("priority_pkey");

                    b.HasIndex("Level")
                        .IsUnique();

                    b.ToTable("priority", (string)null);
                });

            modelBuilder.Entity("Application.Models.ToDoItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<int?>("Priority")
                        .HasColumnType("integer");

                    b.Property<int?>("PriorityNavigationId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("User")
                        .HasColumnType("integer");

                    b.Property<int?>("UserNavigationId")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("todo-item_pkey");

                    b.HasIndex("PriorityNavigationId");

                    b.HasIndex("Title");

                    b.HasIndex("UserNavigationId");

                    b.ToTable("todo-item", (string)null);
                });

            modelBuilder.Entity("Application.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("user_pkey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("Application.Models.ToDoItem", b =>
                {
                    b.HasOne("Application.Models.Priority", "PriorityNavigation")
                        .WithMany("ToDoItems")
                        .HasForeignKey("PriorityNavigationId");

                    b.HasOne("Application.Models.User", "UserNavigation")
                        .WithMany("ToDoItems")
                        .HasForeignKey("UserNavigationId");

                    b.Navigation("PriorityNavigation");

                    b.Navigation("UserNavigation");
                });

            modelBuilder.Entity("Application.Models.Priority", b =>
                {
                    b.Navigation("ToDoItems");
                });

            modelBuilder.Entity("Application.Models.User", b =>
                {
                    b.Navigation("ToDoItems");
                });
#pragma warning restore 612, 618
        }
    }
}
