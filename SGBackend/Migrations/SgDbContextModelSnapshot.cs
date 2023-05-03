﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SGBackend.Entities;

#nullable disable

namespace SGBackend.Migrations
{
    [DbContext(typeof(SgDbContext))]
    partial class SgDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SGBackend.Entities.Artist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MediumId");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("SGBackend.Entities.Medium", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AlbumName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("ExplicitContent")
                        .HasColumnType("boolean");

                    b.Property<string>("LinkToMedium")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MediumSource")
                        .HasColumnType("integer");

                    b.Property<string>("ReleaseDate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LinkToMedium")
                        .IsUnique();

                    b.ToTable("Media");
                });

            modelBuilder.Entity("SGBackend.Entities.MediumImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<int>("height")
                        .HasColumnType("integer");

                    b.Property<string>("imageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MediumId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("SGBackend.Entities.MutualPlaybackEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MutualPlaybackOverviewId")
                        .HasColumnType("uuid");

                    b.Property<long>("PlaybackSecondsUser1")
                        .HasColumnType("bigint");

                    b.Property<long>("PlaybackSecondsUser2")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MutualPlaybackOverviewId");

                    b.HasIndex("MediumId", "MutualPlaybackOverviewId")
                        .IsUnique();

                    b.ToTable("MutualPlaybackEntries");
                });

            modelBuilder.Entity("SGBackend.Entities.MutualPlaybackOverview", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("User1Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("User2Id")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("User2Id");

                    b.HasIndex("User1Id", "User2Id")
                        .IsUnique();

                    b.ToTable("MutualPlaybackOverviews");
                });

            modelBuilder.Entity("SGBackend.Entities.PlaybackRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("PlayedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PlayedSeconds")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MediumId");

                    b.HasIndex("UserId", "MediumId", "PlayedAt")
                        .IsUnique();

                    b.ToTable("PlaybackRecords");
                });

            modelBuilder.Entity("SGBackend.Entities.PlaybackSummary", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LastListened")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("MediumId")
                        .HasColumnType("uuid");

                    b.Property<int>("TotalSeconds")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("MediumId", "UserId")
                        .IsUnique();

                    b.ToTable("PlaybackSummaries");
                });

            modelBuilder.Entity("SGBackend.Entities.State", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("GroupedFetchJobInstalled")
                        .HasColumnType("boolean");

                    b.Property<bool>("InitializedFromTarget")
                        .HasColumnType("boolean");

                    b.Property<bool>("QuartzApplied")
                        .HasColumnType("boolean");

                    b.Property<bool>("RandomUsersGenerated")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("SGBackend.Entities.Stats", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LatestFetch")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("SGBackend.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Language")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SpotifyId")
                        .HasColumnType("text");

                    b.Property<string>("SpotifyProfileUrl")
                        .HasColumnType("text");

                    b.Property<string>("SpotifyRefreshToken")
                        .HasColumnType("text");

                    b.Property<Guid>("StatsId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StatsId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("SGBackend.Entities.Artist", b =>
                {
                    b.HasOne("SGBackend.Entities.Medium", null)
                        .WithMany("Artists")
                        .HasForeignKey("MediumId");
                });

            modelBuilder.Entity("SGBackend.Entities.MediumImage", b =>
                {
                    b.HasOne("SGBackend.Entities.Medium", null)
                        .WithMany("Images")
                        .HasForeignKey("MediumId");
                });

            modelBuilder.Entity("SGBackend.Entities.MutualPlaybackEntry", b =>
                {
                    b.HasOne("SGBackend.Entities.Medium", "Medium")
                        .WithMany()
                        .HasForeignKey("MediumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SGBackend.Entities.MutualPlaybackOverview", "MutualPlaybackOverview")
                        .WithMany("MutualPlaybackEntries")
                        .HasForeignKey("MutualPlaybackOverviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medium");

                    b.Navigation("MutualPlaybackOverview");
                });

            modelBuilder.Entity("SGBackend.Entities.MutualPlaybackOverview", b =>
                {
                    b.HasOne("SGBackend.Entities.User", "User1")
                        .WithMany()
                        .HasForeignKey("User1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SGBackend.Entities.User", "User2")
                        .WithMany()
                        .HasForeignKey("User2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("SGBackend.Entities.PlaybackRecord", b =>
                {
                    b.HasOne("SGBackend.Entities.Medium", "Medium")
                        .WithMany()
                        .HasForeignKey("MediumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SGBackend.Entities.User", "User")
                        .WithMany("PlaybackRecords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medium");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SGBackend.Entities.PlaybackSummary", b =>
                {
                    b.HasOne("SGBackend.Entities.Medium", "Medium")
                        .WithMany()
                        .HasForeignKey("MediumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SGBackend.Entities.User", "User")
                        .WithMany("PlaybackSummaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medium");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SGBackend.Entities.User", b =>
                {
                    b.HasOne("SGBackend.Entities.Stats", "Stats")
                        .WithMany()
                        .HasForeignKey("StatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stats");
                });

            modelBuilder.Entity("SGBackend.Entities.Medium", b =>
                {
                    b.Navigation("Artists");

                    b.Navigation("Images");
                });

            modelBuilder.Entity("SGBackend.Entities.MutualPlaybackOverview", b =>
                {
                    b.Navigation("MutualPlaybackEntries");
                });

            modelBuilder.Entity("SGBackend.Entities.User", b =>
                {
                    b.Navigation("PlaybackRecords");

                    b.Navigation("PlaybackSummaries");
                });
#pragma warning restore 612, 618
        }
    }
}
