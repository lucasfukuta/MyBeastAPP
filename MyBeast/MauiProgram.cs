using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microcharts.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

// Namespaces do Projeto
using MyBeast.Data;
using MyBeast.Services;
using MyBeast.ViewModels;
using MyBeast.ViewModels.Auth;
using MyBeast.ViewModels.Diet;
using MyBeast.ViewModels.Stats;
using MyBeast.ViewModels.Workout;
using MyBeast.Views.Auth;
using MyBeast.Views.Diet;
using MyBeast.Views.Stats;
using MyBeast.Views.Workout;

namespace MyBeast
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseLiveCharts()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // --- 1. BANCO DE DADOS ---
            builder.Services.AddDbContext<LocalDbContext>();

            // --- 2. SERVIÇOS GERAIS (Sempre Singleton) ---
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                // Lógica para definir a URL base dependendo se é Android ou Windows
                string baseUrl;

                // IMPORTANTE: Verifique se a porta (7234) é a mesma do seu launchSettings.json da API
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    baseUrl = "https://10.0.2.2:7261/";
                else
                    baseUrl = "https://localhost:5145/";

                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // Ignora erros de SSL (certificado) no desenvolvimento local (Android/Emulator)
                var handler = new HttpClientHandler();
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }
                return handler;
            });

            // Configuração para DADOS DA API (Dieta, Treinos)
            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                string baseUrl;
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    baseUrl = "https://10.0.2.2:7261/"; // Mesma porta da Auth
                else
                    baseUrl = "https://localhost:5145/";

                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }
                return handler;
            });

            builder.Services.AddSingleton<ILocalDbService, LocalDbService>();
            builder.Services.AddSingleton<IPetService, PetService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            // --- 3. FEATURES DE AUTENTICAÇÃO (Transient) ---
            // Devem ser recriadas para garantir campos limpos ao fazer logout/login
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<RegisterViewModel>();

            builder.Services.AddTransient<ForgotPasswordViewModel>();

            // --- 4. TELAS PRINCIPAIS (Singleton) ---
            // Mantêm o estado enquanto o app roda e escutam mensagens globais.

            // Home / Pet (Precisa ser Singleton para o Pet manter o status)
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();

            // Stats (CRUCIAL: Singleton para ouvir mensagens de treino/dieta em background)
            builder.Services.AddSingleton<StatsPage>();
            builder.Services.AddSingleton<StatsPageViewModel>();

            // Dieta (Singleton para não perder a lista de refeições ao navegar)
            builder.Services.AddSingleton<DietPage>();
            builder.Services.AddSingleton<DietViewModel>();

            // Lista de Treinos (Singleton para manter a lista carregada)
            builder.Services.AddSingleton<WorkoutListPage>();
            builder.Services.AddSingleton<WorkoutListViewModel>();

            // --- 5. SUB-TELAS E FLUXOS (Transient) ---
            // Nascem e morrem a cada navegação.

            // Edição de Dieta (Sempre limpa ao abrir)
            builder.Services.AddTransient<MealEditorPage>();
            builder.Services.AddTransient<MealEditorViewModel>();

            // Detalhes do Treino (Muda dependendo do treino clicado)
            builder.Services.AddTransient<WorkoutDetailPage>();
            builder.Services.AddTransient<WorkoutDetailViewModel>();

            // Cronômetro (Deve zerar sempre que abrir)
            builder.Services.AddTransient<ActiveWorkoutPage>();
            builder.Services.AddTransient<ActiveWorkoutViewModel>();

            // Resumo (Tela final)
            builder.Services.AddTransient<WorkoutSummaryPage>();
            // builder.Services.AddTransient<WorkoutSummaryViewModel>(); // Se tiver ViewModel para essa

            // --- 6. CONSTRUÇÃO E MIGRAÇÃO ---
            var app = builder.Build();

            InitializeDatabase(app);

            return app;
        }

        private static void InitializeDatabase(MauiApp app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
                try
                {
                    // Aplica migrações pendentes e cria o banco se não existir
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DB ERROR] Falha ao migrar banco de dados: {ex.Message}");
                }
            }
        }
    }
}