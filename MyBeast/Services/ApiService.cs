using System.Net.Http.Json;
using MyBeast.Domain.Entities; // <--- CORREÇÃO 1: Importando a Entidade Achievement
using MyBeast.Domain.DTOs.WorkoutTemplate.Output; // DTOs de Saída
using MyBeast.Domain.DTOs.WorkoutTemplate.Input;
using MyBeast.Domain.DTOs.FoodItem.Output;  // DTOs de Entrada

namespace MyBeast.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ApiService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        // ==========================================================
        // ÁREA DE CONQUISTAS (Mantendo sua lógica, mas modernizada)
        // ==========================================================

        public async Task<Achievement> GetAchievementAsync(string url)
        {
            // GetFromJsonAsync já faz o trabalho de desserializar
            return await _httpClient.GetFromJsonAsync<Achievement>(url);
        }

        public async Task<IEnumerable<Achievement>> GetAchievementsAsync(string url)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Achievement>>(url) ?? new List<Achievement>();
        }

        public async Task<bool> PostAchievementAsync(string url, Achievement achievement)
        {
            // CORREÇÃO 2: Substituímos o StringContent manual por PostAsJsonAsync
            var response = await _httpClient.PostAsJsonAsync(url, achievement);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAchievementAsync(string url, Achievement achievement)
        {
            // CORREÇÃO 2: Substituímos o StringContent manual por PutAsJsonAsync
            var response = await _httpClient.PutAsJsonAsync(url, achievement);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAchievementAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        // ==========================================================
        // ÁREA DE TREINOS (WORKOUTS) - Nova implementação com DTOs
        // ==========================================================

        public async Task<List<WorkoutTemplateDto>> GetMyWorkoutsAsync()
        {
            try
            {
                // Aqui você pode ajustar a rota se necessário (ex: passar o userId)
                // Por enquanto assume-se que o token JWT (se houver) define o usuário
                var response = await _httpClient.GetAsync("api/WorkoutTemplates/me");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<WorkoutTemplateDto>>() ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar treinos: {ex.Message}");
            }
            return new List<WorkoutTemplateDto>();
        }

        // ==========================================================
        // ÁREA DE ALIMENTOS (DIET / FOOD ITEMS)
        // ==========================================================

        public async Task<List<FoodItemDto>> GetFoodTemplatesAsync()
        {
            try
            {
                // 1. GARANTE O TOKEN
                await SetAuthorizationHeader();

                // 2. LOG DA TENTATIVA
                Console.WriteLine($"[API] Tentando buscar alimentos em: {_httpClient.BaseAddress}api/FoodItems");

                var response = await _httpClient.GetAsync("api/FoodItems");

                // 3. LOG DO STATUS
                Console.WriteLine($"[API] Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // 4. LER COMO STRING PRIMEIRO (Para ver o que veio)
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[API] JSON Recebido: {jsonResult}");

                    // 5. CONFIGURAÇÃO DO JSON (Para ignorar maiúsculas/minúsculas)
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var dados = System.Text.Json.JsonSerializer.Deserialize<List<FoodItemDto>>(jsonResult, options);
                    Console.WriteLine($"[API] Total deserializado: {dados?.Count ?? 0}");

                    return dados ?? new List<FoodItemDto>();
                }
                else
                {
                    // Se der erro (ex: 401 ou 500), mostra o motivo
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[API ERROR] {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API EXCEPTION] {ex.Message}");
            }

            return new List<FoodItemDto>();
        }

        public async Task<List<WorkoutTemplateDto>> GetDefaultWorkoutsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/WorkoutTemplates/defaults");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<WorkoutTemplateDto>>() ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar padrões: {ex.Message}");
            }
            return new List<WorkoutTemplateDto>();
        }

        public async Task<bool> CreateWorkoutAsync(WorkoutTemplateCreateDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/WorkoutTemplates", createDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteWorkoutAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/WorkoutTemplates/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        private async Task SetAuthorizationHeader()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}