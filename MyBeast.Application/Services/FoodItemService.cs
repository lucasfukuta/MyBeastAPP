using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;

namespace MyBeast.Application.Services
{
    public class FoodItemService : IFoodItemService
    {
        private readonly IFoodItemRepository _foodItemRepository;
        private readonly IUserRepository _userRepository;

        public FoodItemService(IFoodItemRepository foodItemRepository, IUserRepository userRepository)
        {
            _foodItemRepository = foodItemRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<FoodItem>> GetAllFoodItemsAsync(int? userId)
        {
            return await _foodItemRepository.GetAllAccessibleAsync(userId ?? 0);
        }

        public async Task<FoodItem?> GetFoodItemByIdAsync(int id)
        {
            // TODO: Adicionar lógica de permissão se necessário
            return await _foodItemRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<FoodItem>> GetCustomFoodItemsByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            return await _foodItemRepository.GetByUserIdAsync(userId);
        }

        public async Task<FoodItem> CreateCustomFoodItemAsync(FoodItem foodItem, int userId)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(foodItem.Name)) throw new ArgumentException("Nome do alimento é obrigatório.");
            if (foodItem.Calories < 0 || foodItem.Protein < 0 || foodItem.Carbs < 0 || foodItem.Fat < 0)
                throw new ArgumentException("Valores nutricionais não podem ser negativos.");

            // Verificar usuário
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            // Verificar duplicatas (nome + user OU nome + template)
            var existingCustom = await _foodItemRepository.GetByNameAndUserIdAsync(foodItem.Name, userId);
            if (existingCustom != null) throw new Exception($"Usuário já possui um alimento customizado com o nome '{foodItem.Name}'.");
            var existingTemplate = await _foodItemRepository.GetByNameAndUserIdAsync(foodItem.Name, null);
            if (existingTemplate != null) throw new Exception($"Já existe um alimento template com o nome '{foodItem.Name}'.");

            // Definir propriedades
            foodItem.UserId = userId;
            foodItem.IsCustom = true;
            foodItem.User = null; // Limpar navegação
            foodItem.MealLogItems = new List<MealLogItem>(); // Limpar navegação

            return await _foodItemRepository.AddAsync(foodItem);
        }

        public async Task<FoodItem> UpdateCustomFoodItemAsync(int id, FoodItem foodItemUpdateData, int requestingUserId)
        {
            var existingFoodItem = await _foodItemRepository.GetByIdAsync(id);
            if (existingFoodItem == null) throw new Exception($"Alimento com ID {id} não encontrado.");

            // Verificação de Permissão e Tipo
            if (!existingFoodItem.IsCustom) throw new Exception("Não é permitido editar alimentos template.");
            if (existingFoodItem.UserId != requestingUserId) throw new Exception("Usuário não tem permissão para editar este alimento.");

            // Validar e Atualizar campos permitidos
            if (!string.IsNullOrWhiteSpace(foodItemUpdateData.Name) && existingFoodItem.Name != foodItemUpdateData.Name)
            {
                // Verificar NOVO nome duplicado
                var duplicateCheck = await _foodItemRepository.GetByNameAndUserIdAsync(foodItemUpdateData.Name, requestingUserId);
                if (duplicateCheck != null && duplicateCheck.FoodId != id) throw new Exception($"Usuário já possui um alimento customizado com o nome '{foodItemUpdateData.Name}'.");
                var templateCheck = await _foodItemRepository.GetByNameAndUserIdAsync(foodItemUpdateData.Name, null);
                if (templateCheck != null) throw new Exception($"Já existe um alimento template com o nome '{foodItemUpdateData.Name}'.");

                existingFoodItem.Name = foodItemUpdateData.Name;
            }
            // Atualizar Nutrientes (permitir negativos aqui?)
            if (foodItemUpdateData.Calories >= 0) existingFoodItem.Calories = foodItemUpdateData.Calories;
            if (foodItemUpdateData.Protein >= 0) existingFoodItem.Protein = foodItemUpdateData.Protein;
            if (foodItemUpdateData.Carbs >= 0) existingFoodItem.Carbs = foodItemUpdateData.Carbs;
            if (foodItemUpdateData.Fat >= 0) existingFoodItem.Fat = foodItemUpdateData.Fat;

            return await _foodItemRepository.UpdateAsync(existingFoodItem);
        }

        public async Task DeleteCustomFoodItemAsync(int id, int requestingUserId)
        {
            var existingFoodItem = await _foodItemRepository.GetByIdAsync(id);
            if (existingFoodItem == null) throw new Exception($"Alimento com ID {id} não encontrado.");

            // Verificação de Permissão e Tipo
            if (!existingFoodItem.IsCustom) throw new Exception("Não é permitido deletar alimentos template.");
            if (existingFoodItem.UserId != requestingUserId) throw new Exception("Usuário não tem permissão para deletar este alimento.");

            // TODO: Verificar se o alimento está sendo usado em algum MealLogItem antes de deletar?

            await _foodItemRepository.DeleteAsync(id);
        }
    }
}