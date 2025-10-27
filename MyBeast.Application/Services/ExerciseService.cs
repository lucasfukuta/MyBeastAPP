using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;

namespace MyBeast.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;

        // Pedimos o "contrato" do repositório, não a classe concreta
        public ExerciseService(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public async Task<IEnumerable<Exercise>> GetAllExercisesAsync()
        {
            // Aqui podemos adicionar lógica de negócio no futuro
            // (ex: filtrar por tipo de plano, etc.)
            return await _exerciseRepository.GetAllAsync();
        }
    }
}