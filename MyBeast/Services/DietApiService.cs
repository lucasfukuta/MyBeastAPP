using MyBeast.Models.DTOs.Diet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace MyBeast.Services
{
    public class DietApiService : IDietApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalDbService _localDbService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public DietApiService(IHttpClientFactory httpClientFactory, ILocalDbService localDbService)
        {
            _httpClientFactory = httpClientFactory;
            _localDbService = localDbService;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<HttpClient> GetClientAsync()
        {
            var client = _httpClientFactory.CreateClient("MyBeastApi");
            var token = await _localDbService.GetTokenAsync(); // Pega o token salvo
            if (!string.IsNullOrEmpty(token?.Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
            }
            return client;
        }

        public async Task<List<MealLogDto>> GetMealLogsByDateAsync(DateTime date)
        {
            var client = await GetClientAsync();
            var dateString = date.ToString("yyyy-MM-dd");
            try
            {
                // Endpoint baseado no seu MealLogsController.cs
                var response = await client.GetAsync($"api/MealLogs/date/{dateString}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<MealLogDto>>(content, _jsonSerializerOptions) ?? new List<MealLogDto>();
                }
                return new List<MealLogDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar logs de refeição: {ex.Message}");
                return new List<MealLogDto>();
            }
        }

        public async Task<List<FoodItemDto>> GetFoodItemsAsync()
        {
            var client = await GetClientAsync();
            try
            {
                // Endpoint baseado no seu FoodItemsController.cs
                var response = await client.GetAsync("api/FoodItems");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<FoodItemDto>>(content, _jsonSerializerOptions) ?? new List<FoodItemDto>();
                }
                return new List<FoodItemDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar itens de comida: {ex.Message}");
                return new List<FoodItemDto>();
            }
        }
    }
}