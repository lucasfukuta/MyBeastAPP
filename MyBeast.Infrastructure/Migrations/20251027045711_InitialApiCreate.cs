using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBeast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialApiCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MuscleGroup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.ExerciseId);
                });

            migrationBuilder.CreateTable(
                name: "FoodItem",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Calories = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Protein = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Fat = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItem", x => x.FoodId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PlanType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsModerator = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Achievement",
                columns: table => new
                {
                    AchievementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateAchieved = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement", x => x.AchievementId);
                    table.ForeignKey(
                        name: "FK_Achievement_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AISuggestion",
                columns: table => new
                {
                    SuggestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AISuggestion", x => x.SuggestionId);
                    table.ForeignKey(
                        name: "FK_AISuggestion_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityPost",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPost", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_CommunityPost_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealLog",
                columns: table => new
                {
                    MealLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLog", x => x.MealLogId);
                    table.ForeignKey(
                        name: "FK_MealLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    PetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EvolutionLevel = table.Column<int>(type: "int", nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false),
                    Energy = table.Column<int>(type: "int", nullable: false),
                    Hunger = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.PetId);
                    table.ForeignKey(
                        name: "FK_Pets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSession",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    TotalVolume = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSession", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_WorkoutSession_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutTemplate",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTemplate", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_WorkoutTemplate_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "PostReaction",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReactionType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReaction", x => new { x.PostId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PostReaction_CommunityPost_PostId",
                        column: x => x.PostId,
                        principalTable: "CommunityPost",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReaction_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "MealLogItem",
                columns: table => new
                {
                    MealLogId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLogItem", x => new { x.MealLogId, x.FoodId });
                    table.ForeignKey(
                        name: "FK_MealLogItem_FoodItem_FoodId",
                        column: x => x.FoodId,
                        principalTable: "FoodItem",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealLogItem_MealLog_MealLogId",
                        column: x => x.MealLogId,
                        principalTable: "MealLog",
                        principalColumn: "MealLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetLog",
                columns: table => new
                {
                    SetLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    SetNumber = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    RestTimeSeconds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetLog", x => x.SetLogId);
                    table.ForeignKey(
                        name: "FK_SetLog_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercise",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetLog_WorkoutSession_SessionId",
                        column: x => x.SessionId,
                        principalTable: "WorkoutSession",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateExercise",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateExercise", x => new { x.TemplateId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_TemplateExercise_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercise",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateExercise_WorkoutTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "WorkoutTemplate",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_UserId",
                table: "Achievement",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestion_UserId",
                table: "AISuggestion",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPost_UserId",
                table: "CommunityPost",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLog_UserId",
                table: "MealLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogItem_FoodId",
                table: "MealLogItem",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_UserId",
                table: "Pets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_UserId",
                table: "PostReaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SetLog_ExerciseId",
                table: "SetLog",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_SetLog_SessionId",
                table: "SetLog",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateExercise_ExerciseId",
                table: "TemplateExercise",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSession_UserId",
                table: "WorkoutSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutTemplate_UserId",
                table: "WorkoutTemplate",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievement");

            migrationBuilder.DropTable(
                name: "AISuggestion");

            migrationBuilder.DropTable(
                name: "MealLogItem");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "PostReaction");

            migrationBuilder.DropTable(
                name: "SetLog");

            migrationBuilder.DropTable(
                name: "TemplateExercise");

            migrationBuilder.DropTable(
                name: "FoodItem");

            migrationBuilder.DropTable(
                name: "MealLog");

            migrationBuilder.DropTable(
                name: "CommunityPost");

            migrationBuilder.DropTable(
                name: "WorkoutSession");

            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropTable(
                name: "WorkoutTemplate");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
