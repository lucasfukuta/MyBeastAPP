using MyBeast.Domain.Entities;
using MyBeast.Infrastructure.Data; // <--- IMPORTANTE: Aponta para onde está o ApiDbContext
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace MyBeast.API.Services
{
    public class DataSeederService
    {
        // 1. Mude de LocalDbContext para ApiDbContext
        private readonly ApiDbContext _context;
        private readonly HttpClient _httpClient;

        // 2. Atualize o Construtor
        public DataSeederService(ApiDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // --- 1. EXERCÍCIOS (WGER) ---
        public async Task<string> SeedExercisesAsync()
        {
            // Verifica se já tem dados usando o contexto da API
            if (await _context.Exercises.AnyAsync())
                return "Exercícios já existem no banco. Pulando.";

            var exercisesToImport = new List<Exercise>();
            var existingNames = new HashSet<string>();

            // Aumentei o limite para trazer mais dados de uma vez
            string? nextUrl = "https://wger.de/api/v2/exerciseinfo/?limit=60&language=2";

            try
            {
                // Vamos iterar algumas páginas para garantir dados
                while (!string.IsNullOrWhiteSpace(nextUrl) && exercisesToImport.Count < 100)
                {
                    var response = await _httpClient.GetStringAsync(nextUrl);
                    var data = JObject.Parse(response);
                    var results = data["results"] as JArray;

                    if (results != null)
                    {
                        foreach (var item in results)
                        {
                            // Tenta pegar dados diretos ou tradução
                            // A API Wger às vezes muda a estrutura, vamos tentar pegar o nome direto primeiro
                            string name = item["name"]?.ToString();
                            string description = item["description"]?.ToString();

                            // Se for nulo, tenta buscar na lista de traduções (lógica antiga)
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                var translation = item["translations"]?
                                    .FirstOrDefault(t => t["language"] != null && (int)t["language"] == 2);
                                name = translation?["name"]?.ToString();
                                description = translation?["description"]?.ToString();
                            }

                            if (string.IsNullOrWhiteSpace(name) || existingNames.Contains(name)) continue;

                            // Limpar HTML
                            string instructions = Regex.Replace(description ?? "", "<.*?>", String.Empty).Trim();
                            if (string.IsNullOrWhiteSpace(instructions)) instructions = "Sem instruções detalhadas.";

                            // Tratamento do nome
                            if (name.Length > 100) name = name.Substring(0, 100);

                            var exercise = new Exercise
                            {
                                Name = name,
                                Instructions = instructions,
                                MuscleGroup = "Geral", // Simplificação
                                IsCustom = false
                            };

                            exercisesToImport.Add(exercise);
                            existingNames.Add(name);
                        }
                    }

                    nextUrl = data["next"]?.ToString();
                }

                if (exercisesToImport.Any())
                {
                    await _context.Exercises.AddRangeAsync(exercisesToImport);
                    await _context.SaveChangesAsync();
                    return $"Sucesso! {exercisesToImport.Count} exercícios importados.";
                }
                return "Nenhum exercício novo encontrado.";
            }
            catch (Exception ex)
            {
                return $"Erro ao importar exercícios: {ex.Message}";
            }
        }

        // --- 2. ALIMENTOS (USDA) ---
        public async Task<string> SeedFoodsAsync()
        {
            if (await _context.FoodItems.AnyAsync())
                return "Alimentos já existem. Pulando.";

            // Coloque sua API KEY do USDA aqui
            string apiKey = "DEMO_KEY"; // Troque pela sua se tiver erro de limite (429)
            var foodsToImport = new List<FoodItem>();
            var existingNames = new HashSet<string>();

            try
            {
                // Importar 3 páginas (150 itens)
                for (int page = 1; page <= 3; page++)
                {
                    string url = $"https://api.nal.usda.gov/fdc/v1/foods/list?api_key={apiKey}&pageSize=50&pageNumber={page}&dataType=Foundation,SR%20Legacy";

                    var response = await _httpClient.GetStringAsync(url);
                    var items = JArray.Parse(response);

                    foreach (var item in items)
                    {
                        string name = item["description"]?.ToString();
                        if (string.IsNullOrWhiteSpace(name) || existingNames.Contains(name)) continue;

                        if (name.Length > 100) name = name.Substring(0, 100);

                        var food = new FoodItem
                        {
                            Name = name,
                            IsCustom = false,
                            Calories = (int)GetNutrientValue(item, "208"), // Energy (KCAL)
                            Protein = (int)GetNutrientValue(item, "203"), // Protein
                            Fat = (int)GetNutrientValue(item, "204"),     // Total Lipid
                            Carbs = (int)GetNutrientValue(item, "205")    // Carbs
                        };

                        foodsToImport.Add(food);
                        existingNames.Add(name);
                    }
                }

                await _context.FoodItems.AddRangeAsync(foodsToImport);
                await _context.SaveChangesAsync();
                return $"Sucesso! {foodsToImport.Count} alimentos importados.";
            }
            catch (Exception ex)
            {
                return $"Erro ao importar alimentos: {ex.Message}";
            }
        }

        private static decimal GetNutrientValue(JToken foodItem, string nutrientNumber)
        {
            var nutrients = foodItem["foodNutrients"];
            if (nutrients == null) return 0;

            var target = nutrients.FirstOrDefault(n => n["number"]?.ToString() == nutrientNumber);
            if (target != null && decimal.TryParse(target["amount"]?.ToString(), out decimal val))
            {
                return val;
            }
            return 0;
        }
    }
}