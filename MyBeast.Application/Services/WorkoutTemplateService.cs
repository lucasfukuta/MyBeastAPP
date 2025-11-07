using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class WorkoutTemplateService : IWorkoutTemplateService
    {
        private readonly IWorkoutTemplateRepository _templateRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IUserRepository _userRepository;

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
            // Retorna todos (Serviço/Repo deve filtrar ou paginar)
            return await _templateRepository.GetAllAsync();
        }

        // --- MÉTODO ATUALIZADO (com requestingUserId) ---
        public async Task<WorkoutTemplate?> GetWorkoutTemplateByIdAsync(int id, int requestingUserId, bool includeExercises = false)
        {
            var template = await _templateRepository.GetByIdAsync(id, includeExercises);
            if (template == null) return null;

            // VERIFICAÇÃO DE PERMISSÃO:
            // Se o template NÃO é padrão (UserId != null) E o dono NÃO é o usuário logado
            if (template.UserId != null && template.UserId != requestingUserId)
            {
                // TODO: Adicionar lógica de Admin/Moderador se necessário
                throw new Exception("Usuário não tem permissão para ver este template.");
            }
            // Se for padrão (UserId == null) ou pertencer ao usuário, retorna
            return template;
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetDefaultWorkoutTemplatesAsync(bool includeExercises = false)
        {
            return await _templateRepository.GetDefaultsAsync(includeExercises);
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetWorkoutTemplatesByUserIdAsync(int userId, bool includeExercises = false)
        {
            // Verificar se o usuário existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            return await _templateRepository.GetByUserIdAsync(userId, includeExercises);
        }

        // --- MÉTODO ATUALIZADO (com requestingUserId) ---
        public async Task<WorkoutTemplate> CreateWorkoutTemplateAsync(WorkoutTemplate template, int requestingUserId)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(template.Name)) throw new ArgumentException("Nome do template é obrigatório.");

            // Verificar se usuário existe
            var user = await _userRepository.GetByIdAsync(requestingUserId);
            if (user == null) throw new Exception($"Usuário com ID {requestingUserId} não encontrado.");

            // Atribui o UserId do token
            template.UserId = requestingUserId;

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
                template.TemplateExercises = new List<TemplateExercise>();
            }

            template.IsPremium = false; // Templates de usuário não são premium por padrão
            return await _templateRepository.AddAsync(template);
        }

        // --- (Este método já estava correto no seu código) ---
        public async Task<WorkoutTemplate> UpdateWorkoutTemplateAsync(int id, WorkoutTemplate templateUpdateData, int requestingUserId)
        {
            var existingTemplate = await _templateRepository.GetByIdAsync(id, true); // Inclui exercícios para atualizar
            if (existingTemplate == null) throw new Exception($"Template com ID {id} não encontrado.");

            // Verificação de Permissão
            if (existingTemplate.UserId != requestingUserId)
            {
                throw new Exception("Usuário não tem permissão para editar este template.");
            }
            if (existingTemplate.UserId == null)
            {
                throw new Exception("Não é permitido editar templates padrão do aplicativo.");
            }

            // Atualizar campos permitidos
            if (!string.IsNullOrWhiteSpace(templateUpdateData.Name)) existingTemplate.Name = templateUpdateData.Name;
            if (!string.IsNullOrWhiteSpace(templateUpdateData.Difficulty)) existingTemplate.Difficulty = templateUpdateData.Difficulty;

            // Atualizar Exercícios
            if (templateUpdateData.TemplateExercises != null)
            {
                // Abordagem simples: Remove todos os antigos e adiciona os novos
                existingTemplate.TemplateExercises.Clear();

                foreach (var te in templateUpdateData.TemplateExercises.OrderBy(t => t.OrderIndex))
                {
                    var exerciseExists = await _exerciseRepository.GetByIdAsync(te.ExerciseId);
                    if (exerciseExists == null) throw new Exception($"Exercício com ID {te.ExerciseId} não encontrado.");

                    existingTemplate.TemplateExercises.Add(new TemplateExercise
                    {
                        TemplateId = existingTemplate.TemplateId,
                        ExerciseId = te.ExerciseId,
                        OrderIndex = te.OrderIndex
                    });
                }
            }

            return await _templateRepository.UpdateAsync(existingTemplate);
        }

        // --- (Este método já estava correto no seu código) ---
        public async Task DeleteWorkoutTemplateAsync(int id, int requestingUserId)
        {
            var existingTemplate = await _templateRepository.GetByIdAsync(id); // Não precisa de includes para deletar
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