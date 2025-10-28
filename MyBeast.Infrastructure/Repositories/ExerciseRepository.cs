using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic; // Adicionado
using System.Linq; // Adicionado
using System.Threading.Tasks; // Adicionado

namespace MyBeast.Infrastructure.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApiDbContext _context;

        public ExerciseRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync()
        {
            // Retorna todos (templates e customizados)
            // O serviço pode filtrar se necessário
            return await _context.Exercises.AsNoTracking().ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            return await _context.Exercises.AsNoTracking()
                       .FirstOrDefaultAsync(e => e.ExerciseId == id);
        }

        // --- NOVOS MÉTODOS ---
        public async Task<IEnumerable<Exercise>> GetByUserIdAsync(int userId)
        {
            // Retorna apenas exercícios customizados do usuário
            return await _context.Exercises
                       .Where(e => e.IsCustom && e.UserId == userId)
                       .OrderBy(e => e.Name)
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task<Exercise?> GetByNameAndUserIdAsync(string name, int? userId)
        {
            // Busca por nome exato, considerando se é template (userId == null) ou custom
            // Ignora case para evitar duplicatas como "Supino" e "supino"
            return await _context.Exercises.AsNoTracking()
                       .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower() && e.UserId == userId);
        }


        public async Task<Exercise> AddAsync(Exercise exercise)
        {
            // Garante que User não seja inserido/atualizado aqui
            exercise.User = null;

            await _context.Exercises.AddAsync(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> UpdateAsync(Exercise exercise)
        {
            _context.Entry(exercise).State = EntityState.Modified;
            // Garante que IsCustom e UserId não sejam modificados
            _context.Entry(exercise).Property(e => e.IsCustom).IsModified = false;
            _context.Entry(exercise).Property(e => e.UserId).IsModified = false;
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task DeleteAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise != null && exercise.IsCustom) // Só permite deletar customizados
            {
                // Verificar se está em uso em TemplateExercise ou SetLog?
                // A FK constraint deve impedir se estiver em uso, gerando erro.
                // Ou podemos checar manualmente antes.
                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }
            // else // Lançar erro se for template ou não encontrado? O serviço já valida.
        }
        // --- FIM DOS NOVOS MÉTODOS ---
    }
}