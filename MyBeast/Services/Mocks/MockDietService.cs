using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Services.Mocks
{
    public class MockDietService : IDietService
    {
        // Nosso "Banco de Dados" em memória
        private readonly List<MealLog> _fakeDatabase;

        public MockDietService()
        {
            _fakeDatabase = new List<MealLog>();
            SeedData(); // Popula com dados iniciais
        }

        private void SeedData()
        {
            // Cria um registro para o "Almoço" de hoje
            var logAlmoco = new MealLog
            {
                MealLogId = 1,
                Date = DateTime.Today,
                MealType = "Almoço",
                MealLogItems = new List<MealLogItem>
                {
                    new MealLogItem
                    {
                        MealLogItemId = 101,
                        Quantity = 1,
                        FoodItem = new FoodItem
                        {
                            FoodId = 1, Name = "Frango Grelhado", Calories = 165, Protein = 31, Carbs = 0, Fat = 3.6m
                        }
                    },
                    new MealLogItem
                    {
                        MealLogItemId = 102,
                        Quantity = 2, // 200g de Arroz por exemplo
                        FoodItem = new FoodItem
                        {
                            FoodId = 2, Name = "Arroz Branco", Calories = 130, Protein = 2.7m, Carbs = 28, Fat = 0.3m
                        }
                    }
                }
            };

            _fakeDatabase.Add(logAlmoco);
        }

        public async Task<IEnumerable<MealLog>> GetMealsByDateAsync(DateTime date)
        {
            await Task.Delay(500); // Simula delay de rede (loading)

            // Retorna todos os logs daquela data
            return _fakeDatabase.Where(m => m.Date.Date == date.Date).ToList();
        }

        public async Task AddFoodItemAsync(DateTime date, string mealType, FoodItem foodItem, double quantity)
        {
            await Task.Delay(300);

            // 1. Verifica se já existe um log para essa refeição (ex: Almoço) nesta data
            var existingLog = _fakeDatabase.FirstOrDefault(m => m.Date.Date == date.Date && m.MealType == mealType);

            if (existingLog == null)
            {
                existingLog = new MealLog
                {
                    MealLogId = new Random().Next(1000, 9999), // ID falso
                    Date = date,
                    MealType = mealType,
                    MealLogItems = new List<MealLogItem>()
                };
                _fakeDatabase.Add(existingLog);
            }

            // 2. Adiciona o item
            existingLog.MealLogItems.Add(new MealLogItem
            {
                MealLogItemId = new Random().Next(1000, 9999),
                MealLogId = existingLog.MealLogId,
                FoodItem = foodItem,
                Quantity = (decimal)quantity // Casting pois sua entidade usa decimal e a view double
            });
        }

        public async Task RemoveFoodItemAsync(int mealLogItemId)
        {
            await Task.Delay(300);

            foreach (var log in _fakeDatabase)
            {
                var itemToRemove = log.MealLogItems.FirstOrDefault(i => i.MealLogItemId == mealLogItemId);
                if (itemToRemove != null)
                {
                    log.MealLogItems.Remove(itemToRemove);

                    // Se não tem mais itens, removemos o cabeçalho da refeição (opcional)
                    if (!log.MealLogItems.Any())
                    {
                        // Lógica para remover o log vazio se desejar
                    }
                    break;
                }
            }
        }
    }
}
