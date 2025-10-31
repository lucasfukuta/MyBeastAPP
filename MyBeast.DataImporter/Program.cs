using CsvHelper; // Você não precisa mais deste NuGet (CSV)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Adicionar este
using System.Globalization;

Console.WriteLine("Iniciando Ferramenta de Importação...");

// ... (Código de configuração do ApiDbContext - igual ao anterior) ...
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");
var options = new DbContextOptionsBuilder<ApiDbContext>()
    .UseSqlServer(connectionString)
    .Options;
using var context = new ApiDbContext(options);
Console.WriteLine("Conexão com o banco de dados estabelecida.");

// --- 1. IMPORTAR EXERCÍCIOS (API do WGER) ---
try // <--- Outer TRY starts here
{
    if (!await context.Exercises.AnyAsync())
    {
        Console.WriteLine("Importando exercícios da API do WGER...");
        var exercisesToImport = new List<Exercise>();
        // --- DECLARAÇÃO CORRETA do existingNames ---
        var existingNames = new HashSet<string>();
        string? nextUrl = "https://wger.de/api/v2/exerciseinfo/";

        using var client = new HttpClient();

        while (!string.IsNullOrWhiteSpace(nextUrl)) // <--- WHILE loop starts here
        {
            // --- Bloco TRY do WGER (Versão com mais Logs) ---
            try // <--- Inner TRY starts here (inside while)
            {
                Console.WriteLine($"Buscando: {nextUrl}");
                string jsonResponse = await client.GetStringAsync(nextUrl);
                var data = JObject.Parse(jsonResponse);

                var results = data["results"]?.ToObject<JArray>();

                if (results == null || !results.HasValues)
                {
                    Console.WriteLine(" -> Nenhum resultado encontrado nesta página.");
                    nextUrl = data["next"]?.ToString();
                    continue; // Pula para a próxima iteração do while (CORRECT use of continue)
                }

                Console.WriteLine($" -> Encontrados {results.Count} exercícios nesta página.");

                foreach (var item in results) // <--- FOREACH loop starts here
                {
                    // 1. Encontrar a tradução correta (Assumindo ID 2 = Português, como na imagem)
                    int targetLanguageId = 2; // ID para Português no WGER API
                    var translation = item["translations"]? // Verifica se 'translations' existe
                        .FirstOrDefault(t => t["language"] != null && (int)t["language"] == targetLanguageId);

                    // 2. Extrair nome e descrição da tradução encontrada
                    string? name = translation?["name"]?.ToString();
                    string? rawDescription = translation?["description"]?.ToString();

                    // 3. Verificar se os dados essenciais foram encontrados na tradução
                    bool nameMissing = string.IsNullOrWhiteSpace(name);
                    bool descriptionMissing = string.IsNullOrWhiteSpace(rawDescription);

                    if (nameMissing || descriptionMissing)
                    {
                        string missingFields = "";
                        if (nameMissing) missingFields += "Nome (na tradução PT) ";
                        if (descriptionMissing) missingFields += "Descrição (na tradução PT) ";

                        Console.WriteLine($"   -> Pulando exercício (Dados faltando: {missingFields.Trim()}): ID {item["id"]?.ToString()}");
                        Console.WriteLine($"   ---> JSON DO ITEM PULADO: {item.ToString(Formatting.None)}");
                        continue; // Pula para o próximo item do foreach
                    }

                    // 4. Limpar o HTML da descrição (agora que sabemos que não é nula)
                    string instructions = System.Text.RegularExpressions.Regex.Replace(rawDescription ?? "", "<.*?>", String.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(instructions)) // Pula se a descrição SÓ tinha HTML
                    {
                        Console.WriteLine($"   -> Pulando exercício (Descrição vazia após limpar HTML): {name}");
                        continue;
                    }


                    // 5. Extrair Músculos (como antes, mas verificando 'musclesToken' melhor)
                    var musclesToken = item["muscles"];
                    string muscleGroup = "Não especificado";
                    if (musclesToken != null && musclesToken.Type == JTokenType.Array && musclesToken.HasValues)
                    {
                        var muscleNames = musclesToken
                                            .Select(m => m["name"]?.ToString())
                                            .Where(mn => !string.IsNullOrEmpty(mn))
                                            .ToList();
                        if (muscleNames.Any())
                        {
                            muscleGroup = string.Join(", ", muscleNames);
                            // Truncar se necessário
                            if (muscleGroup.Length > 50) muscleGroup = muscleGroup.Substring(0, 50);
                        }
                    }


                    // 6. Verificar Duplicatas e Truncar Nome
                    if (existingNames.Contains(name))
                    {
                        Console.WriteLine($"   -> Pulando exercício (Nome duplicado): {name}");
                        continue;
                    }
                    if (name.Length > 100) name = name.Substring(0, 100);


                    // 7. Criar o objeto Exercise
                    var exercise = new Exercise
                    {
                        Name = name,
                        Instructions = instructions,
                        IsCustom = false,
                        MuscleGroup = muscleGroup
                    };

                    exercisesToImport.Add(exercise);
                    existingNames.Add(name); // Adiciona nome ao rastreador
                    Console.WriteLine($"   -> Adicionado: {name}");


                } // <--- FOREACH loop ends here

                // Pega a URL da próxima página (com verificação dupla)
                nextUrl = data["next"]?.ToString();
                if (string.IsNullOrWhiteSpace(nextUrl))
                {
                    Console.WriteLine(" -> Fim da paginação.");
                    break; // Sai do loop while explicitamente (CORRECT use of break)
                }

            } // <--- Inner TRY ends here (inside while)
            catch (Exception exWhile) // <--- CATCH for inner try (inside while)
            {
                Console.WriteLine($"ERRO durante busca/processamento da página {nextUrl}: {exWhile.Message}. Tentando próxima página...");
                // Se nextUrl for inválido, o while(!IsNullOrWhiteSpace) vai parar
                nextUrl = null; // Força a saída do loop em caso de erro grave na página
            } // <--- CATCH ends here

        } // <--- WHILE loop ends here

        // Salva todos os exercícios encontrados DEPOIS do loop while
        if (exercisesToImport.Any())
        {
            await context.Exercises.AddRangeAsync(exercisesToImport);
            await context.SaveChangesAsync();
            Console.WriteLine($"Importado com sucesso {exercisesToImport.Count} exercícios.");
        }
        else
        {
            Console.WriteLine("Nenhum exercício novo para importar foi encontrado.");
        }
    }
    else
    {
        Console.WriteLine("Exercícios já existem no banco. Pulando.");
    }
} // <--- Outer TRY ends here
catch (Exception ex) // <--- CATCH for outer try
{
    Console.WriteLine($"ERRO GERAL ao importar exercícios: {ex.Message}");
} // <--- CATCH ends here

// --- 2. IMPORTAR ALIMENTOS (API do USDA) ---
try
{
    if (!await context.FoodItems.AnyAsync())
    {
        Console.WriteLine("Importando alimentos da API do USDA (1000 itens)...");
        string apiKey = "nMNXJuAcLd7DGJGffqHkha9X3f2SmdrCmqr1Q1SV";

        // --- Início da Correção ---

        var foodsToImport = new List<FoodItem>();
        // Usamos um HashSet para rastrear nomes de alimentos e evitar duplicatas
        var existingNames = new HashSet<string>();

        using var client = new HttpClient();
        bool hasPrintedNutrients = false;
        // Vamos buscar as primeiras 20 páginas (50 itens por página = 1000 alimentos)
        for (int page = 1; page <= 20; page++)
        {
            Console.WriteLine($"Buscando alimentos: Página {page}/20");
            string url = $"https://api.nal.usda.gov/fdc/v1/foods/list?api_key={apiKey}&pageSize=50&pageNumber={page}&dataType=Foundation,SR%20Legacy";

            string jsonResponse = await client.GetStringAsync(url);
            var items = JArray.Parse(jsonResponse);

            foreach (var item in items)
            {
                string? name = item["description"]?.ToString() ?? null;

                // Pula se o nome for nulo, vazio ou se já o adicionamos
                if (string.IsNullOrWhiteSpace(name) || existingNames.Contains(name))
                {
                    continue;
                }

                // --- CORREÇÃO DO TRUNCAMENTO ---
                // Garante que o nome não passe de 100 caracteres
                if (name.Length > 100)
                {
                    name = name.Substring(0, 100);
                }

                var foodItem = new FoodItem
                {
                    Name = name,
                    IsCustom = false,
                    // --- CHAMADAS CORRIGIDAS ---
                    Calories = GetNutrientValue(item, "208"), // "Energy" (KCAL)
                    Protein = GetNutrientValue(item, "203"), // "Protein"
                    Fat = GetNutrientValue(item, "204"), // "Total lipid (fat)"
                    Carbs = GetNutrientValue(item, "205")  // "Carbohydrate, by difference"
                };

                foodsToImport.Add(foodItem);
                existingNames.Add(name);
            }
        }

        // --- CORREÇÃO PARA MOSTRAR O ERRO INTERNO ---
        // Vamos envolver o SaveChanges em um try-catch para ver o erro real
        try
        {
            await context.FoodItems.AddRangeAsync(foodsToImport);
            await context.SaveChangesAsync();
            Console.WriteLine($"Importado com sucesso {foodsToImport.Count} alimentos.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO CRÍTICO ao salvar alimentos: {ex.Message}");
            // Tenta mostrar a exceção interna, que contém o erro do SQL Server
            if (ex.InnerException != null)
            {
                Console.WriteLine($"--- ERRO INTERNO ---");
                Console.WriteLine(ex.InnerException.Message);
            }
        }
    }
    else
    {
        Console.WriteLine("Alimentos já existem no banco. Pulando.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERRO ao importar alimentos: {ex.Message}");
}

// Função auxiliar para o USDA 
// Função auxiliar para o USDA (VERSÃO FINAL CORRIGIDA)
static decimal GetNutrientValue(JToken foodItem, string nutrientNumber) // ID agora é string
{
    // 1. Verifica se a lista de nutrientes existe
    if (foodItem["foodNutrients"] == null || !foodItem["foodNutrients"].HasValues)
    {
        return 0;
    }

    // 2. Procura pelo nutriente usando a chave "number" e comparando como string
    var nutrient = foodItem["foodNutrients"]
        .FirstOrDefault(n => n["number"] != null && n["number"].ToString() == nutrientNumber);

    // 3. Verifica se encontrou o nutriente e se ele tem um valor em "amount"
    if (nutrient == null || nutrient["amount"] == null)
    {
        return 0;
    }

    // 4. Converte o valor de "amount" para decimal com segurança
    decimal.TryParse(nutrient["amount"].ToString(), out decimal result);
    return result;
}


Console.WriteLine("Importação concluída. Pressione qualquer tecla para sair.");
Console.ReadKey();