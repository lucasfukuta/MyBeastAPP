using Microsoft.EntityFrameworkCore;
using MyBeast.Application.Interfaces; // Importa
using MyBeast.Application.Services; // Importa
using MyBeast.Domain.Interfaces; // Importa
using MyBeast.Infrastructure.Data; // Importa
using MyBeast.Infrastructure.Repositories; // Importa

var builder = WebApplication.CreateBuilder(args);

// --- 1. Adicionar o DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. Registrar Dependências (Serviços e Repositórios) ---
// Exercise 
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();

// Food 
builder.Services.AddScoped<IFoodItemService, FoodItemService>();
builder.Services.AddScoped<IFoodItemRepository, FoodItemRepository>();

// WorkoutTemplate
builder.Services.AddScoped<IWorkoutTemplateService, WorkoutTemplateService>();
builder.Services.AddScoped<IWorkoutTemplateRepository, WorkoutTemplateRepository>();

// User
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Pet
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();

// WorkoutSessionService
builder.Services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
builder.Services.AddScoped<ISetLogRepository, SetLogRepository>();

// MealLog
builder.Services.AddScoped<IMealLogService, MealLogService>();
builder.Services.AddScoped<IMealLogRepository, MealLogRepository>();
builder.Services.AddScoped<IMealLogItemRepository, MealLogItemRepository>();

// Community
builder.Services.AddScoped<ICommunityPostService, CommunityPostService>();
builder.Services.AddScoped<ICommunityPostRepository, CommunityPostRepository>();
builder.Services.AddScoped<IPostReactionRepository, PostReactionRepository>();

// Achievement
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();

// --- 3. Adicionar serviços padrão da API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();