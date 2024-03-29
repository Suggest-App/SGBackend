﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGBackend.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    MediumSource = table.Column<int>(type: "integer", nullable: false),
                    LinkToMedium = table.Column<string>(type: "text", nullable: false),
                    ExplicitContent = table.Column<bool>(type: "boolean", nullable: false),
                    AlbumName = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuartzApplied = table.Column<bool>(type: "boolean", nullable: false),
                    GroupedFetchJobInstalled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LatestFetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artists_Media_MediumId",
                        column: x => x.MediumId,
                        principalTable: "Media",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    imageUrl = table.Column<string>(type: "text", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Media_MediumId",
                        column: x => x.MediumId,
                        principalTable: "Media",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SpotifyId = table.Column<string>(type: "text", nullable: true),
                    SpotifyProfileUrl = table.Column<string>(type: "text", nullable: true),
                    SpotifyRefreshToken = table.Column<string>(type: "text", nullable: true),
                    StatsId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Stats_StatsId",
                        column: x => x.StatsId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MutualPlaybackOverviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User2Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutualPlaybackOverviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MutualPlaybackOverviews_User_User1Id",
                        column: x => x.User1Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MutualPlaybackOverviews_User_User2Id",
                        column: x => x.User2Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaybackRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlayedSeconds = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaybackRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaybackRecords_Media_MediumId",
                        column: x => x.MediumId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaybackRecords_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaybackSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalSeconds = table.Column<int>(type: "integer", nullable: false),
                    LastListened = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaybackSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaybackSummaries_Media_MediumId",
                        column: x => x.MediumId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaybackSummaries_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MutualPlaybackEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaybackSecondsUser1 = table.Column<long>(type: "bigint", nullable: false),
                    PlaybackSecondsUser2 = table.Column<long>(type: "bigint", nullable: false),
                    MutualPlaybackOverviewId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutualPlaybackEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MutualPlaybackEntries_Media_MediumId",
                        column: x => x.MediumId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MutualPlaybackEntries_MutualPlaybackOverviews_MutualPlaybac~",
                        column: x => x.MutualPlaybackOverviewId,
                        principalTable: "MutualPlaybackOverviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artists_MediumId",
                table: "Artists",
                column: "MediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_MediumId",
                table: "Images",
                column: "MediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_LinkToMedium",
                table: "Media",
                column: "LinkToMedium",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MutualPlaybackEntries_MediumId_MutualPlaybackOverviewId",
                table: "MutualPlaybackEntries",
                columns: new[] { "MediumId", "MutualPlaybackOverviewId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MutualPlaybackEntries_MutualPlaybackOverviewId",
                table: "MutualPlaybackEntries",
                column: "MutualPlaybackOverviewId");

            migrationBuilder.CreateIndex(
                name: "IX_MutualPlaybackOverviews_User1Id_User2Id",
                table: "MutualPlaybackOverviews",
                columns: new[] { "User1Id", "User2Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MutualPlaybackOverviews_User2Id",
                table: "MutualPlaybackOverviews",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackRecords_MediumId",
                table: "PlaybackRecords",
                column: "MediumId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackRecords_UserId_MediumId_PlayedAt",
                table: "PlaybackRecords",
                columns: new[] { "UserId", "MediumId", "PlayedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackSummaries_MediumId_UserId",
                table: "PlaybackSummaries",
                columns: new[] { "MediumId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackSummaries_UserId",
                table: "PlaybackSummaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_StatsId",
                table: "User",
                column: "StatsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "MutualPlaybackEntries");

            migrationBuilder.DropTable(
                name: "PlaybackRecords");

            migrationBuilder.DropTable(
                name: "PlaybackSummaries");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "MutualPlaybackOverviews");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Stats");
        }
    }
}
