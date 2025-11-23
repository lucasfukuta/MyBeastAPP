using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    public interface IWorkoutTemplateService
    {
        // Lista todas as fichas disponíveis
        Task<IEnumerable<WorkoutTemplate>> GetAllTemplatesAsync();

        // Obtém detalhes de uma ficha (incluindo exercícios)
        Task<WorkoutTemplate?> GetTemplateByIdAsync(int id);

        // Cria uma nova ficha
        Task<WorkoutTemplate> CreateTemplateAsync(WorkoutTemplate template);

        // Deleta uma ficha
        Task DeleteTemplateAsync(int id);
    }
}