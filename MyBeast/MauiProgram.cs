using Microsoft.Extensions.Logging;
using MyBeast.Data; // Importar o seu DbContext
using CommunityToolkit.Maui; // Importar o Community Toolkit
using Microsoft.EntityFrameworkCore; // Importar o Entity Framework Core
using MyBeast.Services; // Importar seus Serviços
using MyBeast.ViewModels; // Importar seus ViewModels
using MyBeast.Views; // Importar suas Views

namespace MyBeast
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // <-- ADICIONADO: Inicializa o Community Toolkit (para Popups, etc.)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // --- 1. REGISTRAR O BANCO DE DADOS ---
            builder.Services.AddDbContext<LocalDbContext>();

            // --- 2. REGISTRAR SERVIÇOS, VIEWMODELS E VIEWS ---
            // Você precisará adicionar todos os seus serviços, VMs e Views aqui

            // Exemplo de Serviços (Descomente e adicione os seus)
            // builder.Services.AddSingleton<IAuthService, AuthService>();
            // builder.Services.AddSingleton<ILocalDbService, LocalDbService>();

            // Exemplo de ViewModels (Descomente e adicione os seus)
            // builder.Services.AddTransient<Auth.LoginViewModel>();
            // builder.Services.AddTransient<Auth.RegisterViewModel>();
            // ... etc ...

            // Exemplo de Views (Descomente e adicione as suas)
            // builder.Services.AddTransient<Auth.LoginPage>();
            // builder.Services.AddTransient<Auth.RegisterPage>();
            // ... etc ...


            // --- 3. CONSTRUIR O APP ---
            var app = builder.Build();

            // --- 4. APLICAR MIGRAÇÕES DO BANCO DE DADOS ---
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();

                // Aplica qualquer migração pendente no banco de dados.
                // Isso irá criar o banco na primeira vez e atualizá-lo
                // de forma inteligente em versões futuras, sem perder dados.
                try
                {
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // Adicionar um log de erro aqui caso a migração falhe
                    Console.WriteLine($"Erro ao aplicar migrações: {ex.Message}");
                }
            }

            // --- 5. RETORNAR O APP PRONTO ---
            return app;
        }
    }
}