using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Para Any()
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class MealLogService : IMealLogService
    {
        private readonly IMealLogRepository _mealLogRepository;
        private readonly IMealLogItemRepository _mealLogItemRepository;
        private readonly IUserRepository _userRepository; // Para verificar o usuário
        private readonly IFoodItemRepository _foodItemRepository; // Para verificar os alimentos
        private readonly IPetService _petService;

        public MealLogService(
            IMealLogRepository mealLogRepository,
            IMealLogItemRepository mealLogItemRepository,
            IUserRepository userRepository,
            IFoodItemRepository foodItemRepository, // Adiciona IFoodItemRepository
            IPetService petService) 
        {
            _mealLogRepository = mealLogRepository;
            _mealLogItemRepository = mealLogItemRepository;
            _userRepository = userRepository;
            _foodItemRepository = foodItemRepository;
            _petService = petService;
        }

        public async Task<MealLog?> GetMealLogByIdAsync(int mealLogId)
        {
            // Poderíamos incluir MealLogItems aqui
            return await _mealLogRepository.GetByIdAsync(mealLogId);
        }

        public async Task<IEnumerable<MealLog>> GetMealLogsByUserIdAsync(int userId)
        {
            return await _mealLogRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<MealLog>> GetMealLogsByUserIdAndDateAsync(int userId, DateTime date)
        {
            // O repositório deve buscar apenas a data, ignorando a hora
            return await _mealLogRepository.GetByUserIdAndDateAsync(userId, date.Date);
        }

        public async Task<MealLog> LogMealAsync(int userId, DateTime date, string mealType, List<MealLogItem> items)
        {
            // Verificar usuário
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            // Validar dados básicos
            if (string.IsNullOrWhiteSpace(mealType)) throw new ArgumentException("Tipo de refeição é obrigatório.");
            if (items == null || !items.Any()) throw new ArgumentException("Pelo menos um item alimentar é necessário.");

            // Verificar se todos os FoodIds existem (opcional, mas bom para integridade)
            foreach (var item in items)
            {
                var foodExists = await _foodItemRepository.GetByIdAsync(item.FoodId);
                if (foodExists == null) throw new Exception($"Alimento com ID {item.FoodId} não encontrado.");
                if (item.Quantity <= 0) throw new ArgumentException($"Quantidade inválida para o alimento ID {item.FoodId}.");
            }

            // Criar o MealLog principal
            var newMealLog = new MealLog
            {
                UserId = userId,
                Date = date,
                MealType = mealType
            };

            // Salvar o MealLog para obter o ID
            var savedMealLog = await _mealLogRepository.AddAsync(newMealLog);

            // Associar os MealLogItems ao MealLog salvo
            foreach (var item in items)
            {
                item.MealLogId = savedMealLog.MealLogId; // Define a FK
            }
            try
            {
                // 1. Define a lógica de mudança de status
                int healthChange = 0; // Dieta não afeta saúde diretamente?
                int energyChange = +10; // Ganha energia
                int hungerChange = -30; // Sacia a fome

                // 2. Busca o pet atual
                var pet = await _petService.GetPetByUserIdAsync(userId);
                if (pet != null)
                {
                    // 3. Calcula os novos valores
                    int newHealth = pet.Health + healthChange;
                    int newEnergy = pet.Energy + energyChange;
                    int newHunger = pet.Hunger + hungerChange;

                    // 4. Chama o serviço do Pet
                    await _petService.UpdatePetStatusAsync(userId, newHealth, newEnergy, newHunger, "Healthy");
                    await _petService.AddExperienceAsync(userId, 10);
                }
            }
            catch (Exception ex)
            {
                // Logar erro do Pet
                Console.WriteLine($"Erro ao atualizar status do Pet pós-refeição: {ex.Message}");
            }
            // Salvar os itens
            await _mealLogItemRepository.AddRangeAsync(items);

            // Retornar o MealLog com os itens (pode precisar buscar novamente para incluir os itens)
            savedMealLog.MealLogItems = items; // Atribui manualmente por enquanto
            return savedMealLog;
        }

        public async Task DeleteMealLogAsync(int mealLogId)
        {
            var mealLog = await _mealLogRepository.GetByIdAsync(mealLogId);
            if (mealLog == null) throw new Exception($"Registro de refeição com ID {mealLogId} não encontrado.");

            // A exclusão em cascata deve cuidar dos MealLogItems (se configurado no DbContext)
            // Se não, teríamos que chamar _mealLogItemRepository.DeleteItemsByMealLogIdAsync(mealLogId); primeiro.
            await _mealLogRepository.DeleteAsync(mealLogId);
        }

        public async Task<MealLog> UpdateMealLogAsync(int mealLogId, int requestingUserId, DateTime? date, string? mealType, List<MealLogItem>? items)
        {
            // 1. Buscar o MealLog existente (incluindo itens para facilitar a exclusão)
            var existingMealLog = await _mealLogRepository.GetByIdAsync(mealLogId);
            if (existingMealLog == null) throw new Exception($"Registro de refeição com ID {mealLogId} não encontrado.");

            // 2. Verificar Permissão
            if (existingMealLog.UserId != requestingUserId)
            {
                throw new Exception("Usuário não tem permissão para editar este registro de refeição.");
            }

            // 3. Atualizar propriedades do MealLog (se fornecidas)
            if (date.HasValue) existingMealLog.Date = date.Value;
            if (!string.IsNullOrWhiteSpace(mealType)) existingMealLog.MealType = mealType;

            // 4. Atualizar MealLogItems (se fornecidos)
            if (items != null)
            {
                // Validar novos itens (FoodId existe? Quantidade > 0?)
                if (items.Any()) // Só valida se a lista não estiver vazia
                {
                    foreach (var item in items)
                    {
                        var foodExists = await _foodItemRepository.GetByIdAsync(item.FoodId);
                        if (foodExists == null) throw new Exception($"Alimento com ID {item.FoodId} não encontrado.");
                        if (item.Quantity <= 0) throw new ArgumentException($"Quantidade inválida para o alimento ID {item.FoodId}.");
                        item.MealLogId = mealLogId; // Garante a FK correta para os novos itens
                    }
                }

                // Abordagem: Deletar todos os itens antigos e adicionar os novos
                await _mealLogItemRepository.DeleteItemsByMealLogIdAsync(mealLogId);
                if (items.Any()) // Só adiciona se a nova lista não estiver vazia
                {
                    await _mealLogItemRepository.AddRangeAsync(items);
                }
                // Atribui manualmente para o objeto retornado (o GetByIdAsync já buscaria com include)
                existingMealLog.MealLogItems = items;
            }

            // 5. Salvar as alterações no MealLog principal
            // Nota: O SaveChanges do AddRangeAsync/DeleteItems já salva.
            // Precisamos salvar apenas as mudanças no MealLog (Date, MealType).
            var updatedMealLog = await _mealLogRepository.UpdateAsync(existingMealLog);

            // Retorna o MealLog atualizado (pode precisar buscar novamente se UpdateAsync não retornar com Includes)
            // Se GetByIdAsync inclui itens, podemos apenas retornar updatedMealLog
            return await _mealLogRepository.GetByIdAsync(mealLogId) ?? updatedMealLog; // Busca novamente para garantir includes
        }
    }
}