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
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
// (Adicione os outros aqui: IFoodItemService, IFoodItemRepository, etc.)


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