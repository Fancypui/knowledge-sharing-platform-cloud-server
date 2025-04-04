using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace knowledge_sharing_platform_cloud.Data.Migrations
{
    /// <inheritdoc />
    public partial class KnowledgeSharingPlatformDBMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "11000, 1"),
                    email = table.Column<string>(type: "NVARCHAR(320)", maxLength: 320, nullable: false),
                    password = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false),
                    username = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    profile_url = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    stripe_account_id = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    created_time = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()"),
                    salt = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Channel",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "21000, 1"),
                    topic = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    channel_img_url = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    channel_img_background = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    user_id = table.Column<long>(type: "BIGINT", nullable: false),
                    last_post_id = table.Column<long>(type: "BIGINT", nullable: true),
                    subscription_fee = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: true),
                    active_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    stripe_price_id = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    total_member = table.Column<int>(type: "INT", nullable: false),
                    total_post = table.Column<int>(type: "INT", nullable: false),
                    created_time = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channel", x => x.id);
                    table.ForeignKey(
                        name: "FK_Channel_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    member_privilege = table.Column<bool>(type: "bit", nullable: false),
                    channel_id = table.Column<long>(type: "BIGINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.id);
                    table.ForeignKey(
                        name: "FK_Category_Channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "Channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channel_Member",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "BIGINT", nullable: false),
                    channel_id = table.Column<long>(type: "BIGINT", nullable: false),
                    subscription_fee_paid = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channel_Member", x => x.id);
                    table.ForeignKey(
                        name: "FK_Channel_Member_Channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "Channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Channel_Member_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "51000, 1"),
                    title = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    body = table.Column<string>(type: "TEXT", nullable: false),
                    post_img_url = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    user_id = table.Column<long>(type: "BIGINT", nullable: false),
                    category_id = table.Column<long>(type: "BIGINT", nullable: false),
                    deleted_status = table.Column<bool>(type: "bit", nullable: false),
                    created_time = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_time = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.id);
                    table.ForeignKey(
                        name: "FK_Post_Category_category_id",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "BIGINT", nullable: false),
                    post_id = table.Column<long>(type: "BIGINT", nullable: false),
                    parent_id = table.Column<long>(type: "BIGINT", nullable: false),
                    root_id = table.Column<long>(type: "BIGINT", nullable: false),
                    comment_content = table.Column<string>(type: "TEXT", nullable: true),
                    created_time = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comment_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "61000, 1"),
                    post_id = table.Column<long>(type: "BIGINT", nullable: false),
                    user_id = table.Column<long>(type: "BIGINT", nullable: false),
                    like_status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Likes_Post_post_id",
                        column: x => x.post_id,
                        principalTable: "Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_channel_id",
                table: "Category",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_Channel_user_id",
                table: "Channel",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Channel_Member_channel_id",
                table: "Channel_Member",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_Channel_Member_user_id",
                table: "Channel_Member",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_post_id",
                table: "Comment",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_post_id",
                table: "Likes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Likes_UserId_PostId",
                table: "Likes",
                columns: new[] { "user_id", "post_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_category_id",
                table: "Post",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_user_id",
                table: "Post",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                table: "User",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channel_Member");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Channel");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
