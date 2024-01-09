﻿// <auto-generated />
using System;
using DBUtilityHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DBUtilityHub.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240105073618_intial1d2")]
    partial class intial1d2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DBUtilityHub.Models.ColumnMetaDataEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ColumnName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Datatype")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DateMaxValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DateMinValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DefaultValue")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("EntityId")
                        .HasColumnType("integer");

                    b.Property<string>("False")
                        .HasColumnType("text");

                    b.Property<bool>("IsForeignKey")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsNullable")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPrimaryKey")
                        .HasColumnType("boolean");

                    b.Property<int?>("Length")
                        .HasColumnType("integer");

                    b.Property<int?>("MaxLength")
                        .HasColumnType("integer");

                    b.Property<int?>("MaxRange")
                        .HasColumnType("integer");

                    b.Property<int?>("MinLength")
                        .HasColumnType("integer");

                    b.Property<int?>("MinRange")
                        .HasColumnType("integer");

                    b.Property<int?>("ReferenceColumnID")
                        .HasColumnType("integer");

                    b.Property<int?>("ReferenceEntityID")
                        .HasColumnType("integer");

                    b.Property<string>("True")
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("ColumnMetaDataEntity");
                });

            modelBuilder.Entity("DBUtilityHub.Models.LogChild", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("ErrorMessage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ErrorRowNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Filedata")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ParentID")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("ParentID");

                    b.ToTable("LogChilds");
                });

            modelBuilder.Entity("DBUtilityHub.Models.LogParent", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<int>("Entity_Id")
                        .HasColumnType("integer");

                    b.Property<int>("FailCount")
                        .HasColumnType("integer");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PassCount")
                        .HasColumnType("integer");

                    b.Property<int>("RecordCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("User_Id")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("LogParents");
                });

            modelBuilder.Entity("DBUtilityHub.Models.RoleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RoleEntity");
                });

            modelBuilder.Entity("DBUtilityHub.Models.TableMetaDataEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DatabaseName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HostName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("TableMetaDataEntity");
                });

            modelBuilder.Entity("DBUtilityHub.Models.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly>("DOB")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phonenumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("UserEntity");
                });

            modelBuilder.Entity("DBUtilityHub.Models.LogChild", b =>
                {
                    b.HasOne("DBUtilityHub.Models.LogParent", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");
                });
#pragma warning restore 612, 618
        }
    }
}
