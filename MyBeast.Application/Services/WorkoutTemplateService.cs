using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using System; // Para Exception
using System.Collections.Generic;
using System.Linq; // Para Any()
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class WorkoutTemplateService : IWorkoutTemplateService
    {
        private readonly IWorkoutTemplateRepository _templateRepository;
        private readonly IExerciseRepository _exerciseRepository; // Para validar exercícios no template
        private readonly IUserRepository _userRepository; // Para validar usuário

        public WorkoutTemplateService(
            IWorkoutTemplateRepository templateRepository,
            IExerciseRepository exerciseRepository,
            IUserRepository userRepository)
        {
            _templateRepository = templateRepository;
            _exerciseRepository = exerciseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesAsync()
        {
            return await _templateRepository.GetAllAsync();
        }

        public async Task<WorkoutTemplate?> GetWorkoutTemplateByIdAsync(int id, bool includeExercises = false)
        {
            return await _templateRepository.GetByIdAsync(id, includeExercises);
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetDefaultWorkoutTemplatesAsync(bool includeExercises = false)
        {
            return await _templateRepository.GetDefaultsAsync(includeExercises);
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetWorkoutTemplatesByUserIdAsync(int userId, bool includeExercises = false)
        {
            // Verificar se o usuário existe (opcional, mas bom)
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            return await _templateRepository.GetByUserIdAsync(userId, includeExercises);
        }

        public async Task<WorkoutTemplate> CreateWorkoutTemplateAsync(WorkoutTemplate template)
        {
            // Validações
            if (template.UserId == null || template.UserId <= 0) throw new ArgumentException("UserId é obrigatório para templates de usuário.");
            if (string.IsNullOrWhiteSpace(template.Name)) throw new ArgumentException("Nome do template é obrigatório.");

            // Verificar se usuário existe
            var user = await _userRepository.GetByIdAsync(template.UserId.Value);
            if (user == null) throw new Exception($"Usuário com ID {template.UserId} não encontrado.");

            // Validar Exercícios (se vierem na criação)
            if (template.TemplateExercises != null && template.TemplateExercises.Any())
            {
                foreach (var te in template.TemplateExercises)
                {
                    var exerciseExists = await _exerciseRepository.GetByIdAsync(te.ExerciseId);
                    if (exerciseExists == null) throw new Exception($"Exercício com ID {te.ExerciseId} não encontrado.");
                }
            }
            else
            {
                // Garante que a coleção não seja nula se não vier nada
                template.TemplateExercises = new List<TemplateExercise>();
            }

            template.IsPremium = false; // Templates de usuário não são premium por padrão
            // Difficulty pode ser opcional ou validado

            return await _templateRepository.AddAsync(template);
        }

        public async Task<WorkoutTemplate> UpdateWorkoutTemplateAsync(int id, WorkoutTemplate templateUpdateData, int requestingUserId)
        {
            var existingTemplate = await _templateRepository.GetByIdAsync(id, true); // Inclui exercícios para atualizar
            if (existingTemplate == null) throw new Exception($"Template com ID {id} não encontrado.");

            // Verificação de Permissão: Usuário só pode editar seus próprios templates
            if (existingTemplate.UserId != requestingUserId)
            {
                throw new Exception("Usuário não tem permissão para editar este template.");
            }
            // Não permitir edição de templates padrão (UserId == null)
            if (existingTemplate.UserId == null)
            {
                throw new Exception("Não é permitido editar templates padrão do aplicativo.");
            }


            // Atualizar campos permitidos
            if (!string.IsNullOrWhiteSpace(templateUpdateData.Name)) existingTemplate.Name = templateUpdateData.Name;
            if (!string.IsNullOrWhiteSpace(templateUpdateData.Difficulty)) existingTemplate.Difficulty = templateUpdateData.Difficulty;

            // Atualizar Exercícios (Lógica mais complexa: remover antigos, adicionar novos)
            if (templateUpdateData.TemplateExercises != null)
            {
                // Remover exercícios que não estão mais na lista (simplificado: remove todos e adiciona os novos)
                existingTemplate.TemplateExercises.Clear(); // Requer que GetByIdAsync inclua os exercícios

                // Validar e adicionar novos exercícios
                foreach (var te in templateUpdateData.TemplateExercises.OrderBy(t => t.OrderIndex)) // Garante ordem
                {
                    var exerciseExists = await _exerciseRepository.GetByIdAsync(te.ExerciseId);
                    if (exerciseExists == null) throw new Exception($"Exercício com ID {te.ExerciseId} não encontrado.");
                    existingTemplate.TemplateExercises.Add(new TemplateExercise
                    {
                        TemplateId = existingTemplate.TemplateId, // Redundante se EF Core gerencia, mas seguro
                        ExerciseId = te.ExerciseId,
                        OrderIndex = te.OrderIndex
                    });
                }
            }

            return await _templateRepository.UpdateAsync(existingTemplate);
        }

        public async Task DeleteWorkoutTemplateAsync(int id, int requestingUserId)
        {
            var existingTemplate = await _templateRepository.GetByIdAsync(id);
            if (existingTemplate == null) throw new Exception($"Template com ID {id} não encontrado.");

            // Verificação de Permissão
            if (existingTemplate.UserId != requestingUserId)
            {
                throw new Exception("Usuário não tem permissão para deletar este template.");
            }
            if (existingTemplate.UserId == null)
            {
                throw new Exception("Não é permitido deletar templates padrão do aplicativo.");
            }

            await _templateRepository.DeleteAsync(id);
        }
    }
}