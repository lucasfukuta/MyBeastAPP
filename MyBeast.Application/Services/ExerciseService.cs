using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using System; // Para Exception
using System.Collections.Generic;
using System.Linq; // Para Any/Where
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IUserRepository _userRepository; // Para validar usuário

        public ExerciseService(IExerciseRepository exerciseRepository, IUserRepository userRepository) // Adiciona IUserRepository
        {
            _exerciseRepository = exerciseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Exercise>> GetAllExercisesAsync(int? userId = null)
        {
            // Retorna templates E customizados do usuário (se userId fornecido)
            var allExercises = await _exerciseRepository.GetAllAsync();
            if (userId.HasValue)
            {
                return allExercises.Where(e => !e.IsCustom || e.UserId == userId.Value);
            }
            // Por padrão, talvez retornar apenas templates? Ou requerer autenticação?
            return allExercises.Where(e => !e.IsCustom); // Exemplo: Retorna só templates se não autenticado
        }

        public async Task<Exercise?> GetExerciseByIdAsync(int id)
        {
            // TODO: Adicionar lógica de permissão se necessário (pode ver custom de outros?)
            return await _exerciseRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Exercise>> GetCustomExercisesByUserIdAsync(int userId)
        {
            // Valida usuário (opcional mas bom)
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            return await _exerciseRepository.GetByUserIdAsync(userId);
        }

        public async Task<Exercise> CreateCustomExerciseAsync(Exercise exercise, int userId)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(exercise.Name)) throw new ArgumentException("Nome do exercício é obrigatório.");
            if (string.IsNullOrWhiteSpace(exercise.MuscleGroup)) throw new ArgumentException("Grupo Muscular é obrigatório.");

            // Verificar se usuário existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            // Verificar se já existe um exercício (template ou custom do user) com mesmo nome
            var existingExercise = await _exerciseRepository.GetByNameAndUserIdAsync(exercise.Name, userId);
            if (existingExercise != null) throw new Exception($"Usuário já possui um exercício customizado com o nome '{exercise.Name}'.");
            var existingTemplate = await _exerciseRepository.GetByNameAndUserIdAsync(exercise.Name, null);
            if (existingTemplate != null) throw new Exception($"Já existe um exercício template com o nome '{exercise.Name}'.");

            // Definir propriedades
            exercise.UserId = userId;
            exercise.IsCustom = true;
            // Limpar navegações que não devem ser salvas aqui
            exercise.User = null;
            exercise.TemplateExercises = new List<TemplateExercise>();
            exercise.SetLogs = new List<SetLog>();


            return await _exerciseRepository.AddAsync(exercise);
        }

        public async Task<Exercise> UpdateCustomExerciseAsync(int id, Exercise exerciseUpdateData, int requestingUserId)
        {
            var existingExercise = await _exerciseRepository.GetByIdAsync(id);
            if (existingExercise == null) throw new Exception($"Exercício com ID {id} não encontrado.");

            // Verificação de Permissão e Tipo
            if (!existingExercise.IsCustom) throw new Exception("Não é permitido editar exercícios template.");
            if (existingExercise.UserId != requestingUserId) throw new Exception("Usuário não tem permissão para editar este exercício.");

            // Validar e Atualizar campos permitidos
            if (!string.IsNullOrWhiteSpace(exerciseUpdateData.Name) && existingExercise.Name != exerciseUpdateData.Name)
            {
                // Verificar se o NOVO nome já existe (para este user ou template)
                var duplicateCheck = await _exerciseRepository.GetByNameAndUserIdAsync(exerciseUpdateData.Name, requestingUserId);
                if (duplicateCheck != null && duplicateCheck.ExerciseId != id) throw new Exception($"Usuário já possui um exercício customizado com o nome '{exerciseUpdateData.Name}'.");
                var templateCheck = await _exerciseRepository.GetByNameAndUserIdAsync(exerciseUpdateData.Name, null);
                if (templateCheck != null) throw new Exception($"Já existe um exercício template com o nome '{exerciseUpdateData.Name}'.");

                existingExercise.Name = exerciseUpdateData.Name;
            }
            if (!string.IsNullOrWhiteSpace(exerciseUpdateData.MuscleGroup)) existingExercise.MuscleGroup = exerciseUpdateData.MuscleGroup;
            // Atualizar Instructions (permitir nulo/vazio?)
            existingExercise.Instructions = exerciseUpdateData.Instructions;

            return await _exerciseRepository.UpdateAsync(existingExercise);
        }

        public async Task DeleteCustomExerciseAsync(int id, int requestingUserId)
        {
            var existingExercise = await _exerciseRepository.GetByIdAsync(id);
            if (existingExercise == null) throw new Exception($"Exercício com ID {id} não encontrado.");

            // Verificação de Permissão e Tipo
            if (!existingExercise.IsCustom) throw new Exception("Não é permitido deletar exercícios template.");
            if (existingExercise.UserId != requestingUserId) throw new Exception("Usuário não tem permissão para deletar este exercício.");

            // TODO: Verificar se o exercício está sendo usado em algum WorkoutTemplate do usuário antes de deletar?

            await _exerciseRepository.DeleteAsync(id);
        }
    }
}