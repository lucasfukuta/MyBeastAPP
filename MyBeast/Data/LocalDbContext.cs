using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Entities;
using MyBeast.Domain.Models; // Importa seus modelos
using System.IO;

namespace MyBeast.Data
{
    public class LocalDbContext : DbContext
    {
        // Mapeia todas as suas classes para tabelas
        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<SetLog> SetLogs { get; set; }
        public DbSet<MealLog> MealLogs { get; set; }
        public DbSet<MealLogItem> MealLogItems { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<TemplateExercise> TemplateExercises { get; set; }
        public DbSet<AISuggestion> AISuggestions { get; set; }

        private readonly string _databasePath;

        public LocalDbContext()
        {
            // Define o caminho para o arquivo do banco de dados SQLite
            string dbName = "MyBeast.db3";
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, dbName);
        }

        // Configura o EF Core para usar o SQLite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
        }

        // Configura as chaves primárias compostas que você definiu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chave composta para TemplateExercise 
            modelBuilder.Entity<TemplateExercise>()
                .HasKey(te => new { te.TemplateId, te.ExerciseId });

            // Chave composta para MealLogItem 
            modelBuilder.Entity<MealLogItem>()
                .HasKey(mli => new { mli.MealLogId, mli.FoodId });

            // Chave composta para PostReaction 
            modelBuilder.Entity<PostReaction>()
                .HasKey(pr => new { pr.PostId, pr.UserId });

            // Configura a relação 1:1 entre User e Pet
            modelBuilder.Entity<User>()
                .HasOne(u => u.Pet)
                .WithOne(p => p.User)
                .HasForeignKey<Pet>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Deleta o Pet se o User for deletado 
        }
    }
}