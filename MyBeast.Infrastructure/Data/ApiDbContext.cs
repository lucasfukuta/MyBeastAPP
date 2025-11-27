using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Entities;
using MyBeast.Application.Interfaces; // Onde está a interface IApplicationDbContext

namespace MyBeast.Infrastructure.Data
{
    public class ApiDbContext : DbContext, IApplicationDbContext
    {
        // --- CONSTRUTOR ÚNICO E CORRETO ---
        // O EF Core usa este construtor para injetar as opções (Connection String)
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        // --- DBSETS (Tabelas) ---
        // Eles implementam a interface IApplicationDbContext automaticamente
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

        // --- CONFIGURAÇÕES DO MODELO (RELACIONAMENTOS) ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. CHAVES COMPOSTAS (N:N) ---

            // PostReaction (Muitos usuários reagem a Muitos posts)
            modelBuilder.Entity<PostReaction>()
                .HasKey(pr => new { pr.PostId, pr.UserId });

            // TemplateExercise (Um treino tem muitos exercícios e vice-versa)
            modelBuilder.Entity<TemplateExercise>()
                .HasKey(te => new { te.TemplateId, te.ExerciseId });

            // MealLogItem (Uma refeição tem muitos alimentos)
            modelBuilder.Entity<MealLogItem>()
                .HasKey(mli => new { mli.MealLogId, mli.FoodId });


            // --- 2. CONFIGURAÇÃO DE CASCATA (EVITAR LOOPS) ---

            // PostReaction: Se apagar o POST, apaga as reações (Cascade)
            // Se apagar o USUÁRIO, apaga as reações SEM cascata automática (NoAction) para evitar ciclo
            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.CommunityPost)
                .WithMany(p => p.PostReactions)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PostReactions)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // --- 3. RELACIONAMENTOS 1:N COM USUÁRIO ---

            // Usuário -> Exercícios Customizados
            modelBuilder.Entity<User>()
                .HasMany(u => u.Exercises)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Evita ciclo ao deletar usuário

            // Usuário -> Alimentos Customizados
            modelBuilder.Entity<User>()
                .HasMany(u => u.FoodItems)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Evita ciclo


            // --- 4. RELAÇÃO 1:1 (USER/PET) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Pet)
                .WithOne(p => p.User)
                .HasForeignKey<Pet>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // Implementação do método da interface (se não herdar de DbContext já)
        // Como herdamos de DbContext, o SaveChangesAsync já existe, mas podemos explicitar se precisar.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}