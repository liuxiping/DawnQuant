﻿// <auto-generated />
using DawnQuant.AShare.Repository.Impl.StrategyMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DawnQuant.AShare.Api.Migrations.StrategyDb
{
    [DbContext(typeof(StrategyDbContext))]
    partial class StrategyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.FactorMetadata", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint");

                    b.Property<string>("Desc")
                        .HasColumnType("longtext");

                    b.Property<string>("ImplAssemblyName")
                        .HasColumnType("longtext");

                    b.Property<string>("ImplClassName")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("ParameterAssemblyName")
                        .HasColumnType("longtext");

                    b.Property<string>("ParameterClassName")
                        .HasColumnType("longtext");

                    b.Property<int>("SortNum")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("FactorMetadatas");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.FactorMetadataCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Desc")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<int>("SortNum")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("FactorMetadataCategories");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.SelectScopeMetadata", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint");

                    b.Property<string>("Desc")
                        .HasColumnType("longtext");

                    b.Property<string>("ImplAssemblyName")
                        .HasColumnType("longtext");

                    b.Property<string>("ImplClassName")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("ParameterAssemblyName")
                        .HasColumnType("longtext");

                    b.Property<string>("ParameterClassName")
                        .HasColumnType("longtext");

                    b.Property<int>("SortNum")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("SelectScopeMetadatas");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.SelectScopeMetadataCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Desc")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<int>("SortNum")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SelectScopeMetadataCategories");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.FactorMetadata", b =>
                {
                    b.HasOne("DawnQuant.AShare.Entities.StrategyMetadata.FactorMetadataCategory", "Category")
                        .WithMany("FactorMetadatas")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.SelectScopeMetadata", b =>
                {
                    b.HasOne("DawnQuant.AShare.Entities.StrategyMetadata.SelectScopeMetadataCategory", "Category")
                        .WithMany("SelectScopeMetadatas")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.FactorMetadataCategory", b =>
                {
                    b.Navigation("FactorMetadatas");
                });

            modelBuilder.Entity("DawnQuant.AShare.Entities.StrategyMetadata.SelectScopeMetadataCategory", b =>
                {
                    b.Navigation("SelectScopeMetadatas");
                });
#pragma warning restore 612, 618
        }
    }
}
