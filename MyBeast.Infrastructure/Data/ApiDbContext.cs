using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Entities;

namespace MyBeast.Infrastructure.Data
{
    public class ApiDbContext : DbContext
    {
        // Construtor usado pela Injeção de Dependência da API
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public ApiDbContext(DbSet<User> users, 
                            DbSet<Pet> pets, 
                            DbSet<Exercise> exercises, 
                            DbSet<FoodItem> foodItems, 
                            DbSet<WorkoutTemplate> workoutTemplates, 
                            DbSet<WorkoutSession> workoutSessions, 
                            DbSet<SetLog> setLogs, 
                            DbSet<MealLog> mealLogs, 
                            DbSet<MealLogItem> mealLogItems, 
                            DbSet<CommunityPost> communityPosts, 
                            DbSet<PostReaction> postReactions, 
                            DbSet<Achievement> achievements, 
                            DbSet<TemplateExercise> templateExercises, 
                            DbSet<AISuggestion> aISuggestions)
        {
            Users = users;
            Pets = pets;
            Exercises = exercises;
            FoodItems = foodItems;
            WorkoutTemplates = workoutTemplates;
            WorkoutSessions = workoutSessions;
            SetLogs = setLogs;
            MealLogs = mealLogs;
            MealLogItems = mealLogItems;
            CommunityPosts = communityPosts;
            PostReactions = postReactions;
            Achievements = achievements;
            TemplateExercises = templateExercises;
            AISuggestions = aISuggestions;
        }

        // Mapeia todas as suas classes de modelo
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. DEFINIÇÃO DAS CHAVES PRIMÁRIAS COMPOSTAS ---

            // ESTA É A LINHA QUE ESTÁ FALTANDO E CAUSANDO O ERRO:
            modelBuilder.Entity<PostReaction>()
                .HasKey(pr => new { pr.PostId, pr.UserId });

            // Adicione as outras chaves compostas (você já deve ter)
            modelBuilder.Entity<TemplateExercise>()
                .HasKey(te => new { te.TemplateId, te.ExerciseId });

            modelBuilder.Entity<MealLogItem>()
                .HasKey(mli => new { mli.MealLogId, mli.FoodId });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Exercises) 
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
            .HasMany(u => u.FoodItems) // Assumindo ICollection<FoodItem> FoodItems no Model User
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            // --- 2. CORREÇÃO DO CAMINHO EM CASCATA ---
            // (Este é o código da etapa anterior, para garantir que continue correto)
            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.CommunityPost)
                .WithMany(p => p.PostReactions)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Deletar reações se o post for deletado

            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PostReactions)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.NoAction); // <-- A correção da cascata

            // --- 3. RELAÇÃO 1:1 (USER/PET) ---
            // (Esta relação também deve estar aqui)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Pet)
                .WithOne(p => p.User)
                .HasForeignKey<Pet>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}