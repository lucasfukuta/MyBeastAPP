using Microsoft.Extensions.Logging;
using MyBeast.Data; // Importar o seu DbContext
using CommunityToolkit.Maui; // Importar o Community Toolkit
using Microsoft.EntityFrameworkCore; // Importar o Entity Framework Core
using MyBeast.Services; // Importar seus Serviços
using MyBeast.ViewModels.Auth;
using MyBeast.ViewModels;
using MyBeast.Views.Auth;
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
            builder.Services.AddHttpClient("MyBeastApi", client =>
            {
                // Usa HTTP (http://) para evitar problemas de certificado SSL
                // Esta URL vem do seu arquivo 'launchSettings.json' da API
                client.BaseAddress = new Uri("http://10.0.2.2:5145");
            });

            // --- 1. REGISTRAR O BANCO DE DADOS ---
            builder.Services.AddDbContext<LocalDbContext>();

            // --- 2. REGISTRAR SERVIÇOS, VIEWMODELS E VIEWS ---
            // Serviços
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ILocalDbService, LocalDbService>();
            builder.Services.AddSingleton<IPetService, PetService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<MainPage>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();

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