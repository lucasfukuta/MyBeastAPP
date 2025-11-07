using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic; // Adicionado
using System.Linq; // Adicionado
using System.Threading.Tasks; // Adicionado

namespace MyBeast.Infrastructure.Repositories
{
    public class WorkoutTemplateRepository : IWorkoutTemplateRepository
    {
        private readonly ApiDbContext _context;

        public WorkoutTemplateRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetAllAsync()
        {
            // Geralmente não inclui exercícios aqui para performance
            return await _context.WorkoutTemplates.AsNoTracking().ToListAsync();
        }

        public async Task<WorkoutTemplate?> GetByIdAsync(int id, bool includeExercises = false)
        {
            var query = _context.WorkoutTemplates.AsQueryable();

            if (includeExercises)
            {
                query = query.Include(wt => wt.TemplateExercises.OrderBy(te => te.OrderIndex)) // Ordena aqui
                             .ThenInclude(te => te.Exercise);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(wt => wt.TemplateId == id);
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetDefaultsAsync(bool includeExercises = false)
        {
            var query = _context.WorkoutTemplates.Where(wt => wt.UserId == null).AsQueryable();

            if (includeExercises)
            {
                query = query.Include(wt => wt.TemplateExercises.OrderBy(te => te.OrderIndex))
                             .ThenInclude(te => te.Exercise);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetByUserIdAsync(int userId, bool includeExercises = false)
        {
            var query = _context.WorkoutTemplates.Where(wt => wt.UserId == userId).AsQueryable();

            if (includeExercises)
            {
                query = query.Include(wt => wt.TemplateExercises.OrderBy(te => te.OrderIndex))
                             .ThenInclude(te => te.Exercise);
            }

            return await query.OrderBy(wt => wt.Name).AsNoTracking().ToListAsync();
        }

        public async Task<WorkoutTemplate> AddAsync(WorkoutTemplate template)
        {
            // Garante que entidades relacionadas não sejam adicionadas incorretamente
            template.User = null;
            if (template.TemplateExercises != null)
            {
                foreach (var te in template.TemplateExercises)
                {
                    te.Exercise = null; // Evita tentar criar exercício
                    te.WorkoutTemplate = null; // Evita referência circular
                }
            }

            await _context.WorkoutTemplates.AddAsync(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<WorkoutTemplate> UpdateAsync(WorkoutTemplate template)
        {
            // Lógica para lidar com a atualização de entidades filho (TemplateExercises)
            // Abordagem 1: Remover existentes e adicionar novos (requer que GetById carregou os filhos)
            _context.TemplateExercises.RemoveRange(template.TemplateExercises.Where(te => te.TemplateId == template.TemplateId)); // Remove antigos se GetById carregou

            // Abordagem 2 (Melhor): O serviço já limpou e adicionou à coleção existente
            // Apenas marcamos o pai como modificado e deixamos o EF Core lidar
            _context.Entry(template).State = EntityState.Modified;

            // Garante que o UserId não seja modificado
            _context.Entry(template).Property(p => p.UserId).IsModified = false;

            await _context.SaveChangesAsync();
            return template;
        }

        public async Task DeleteAsync(int id)
        {
            var template = await _context.WorkoutTemplates.FindAsync(id);
            if (template != null)
            {
                // Se não houver cascata definida no DB, remover filhos manualmente
                // var exercises = await _context.TemplateExercises.Where(te => te.TemplateId == id).ToListAsync();
                // _context.TemplateExercises.RemoveRange(exercises);

                _context.WorkoutTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }
    }
}