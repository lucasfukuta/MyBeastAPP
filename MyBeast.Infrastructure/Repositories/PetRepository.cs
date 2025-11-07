using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.Infrastructure.Data;
using System.Threading.Tasks; // Verifique este using

namespace MyBeast.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository // Garante que implementa a interface
    {
        private readonly ApiDbContext _context;

        public PetRepository(ApiDbContext context)
        {
            _context = context;
        }

        // --- IMPLEMENTAÇÕES ---
        public async Task<Pet?> GetByUserIdAsync(int userId) // async Task<Pet?>
        {
            return await _context.Pets.AsNoTracking()
                       .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Pet?> GetByIdAsync(int petId) // async Task<Pet?>
        {
            return await _context.Pets.AsNoTracking()
                       .FirstOrDefaultAsync(p => p.PetId == petId);
        }

        public async Task<Pet> AddAsync(Pet pet) // async Task<Pet>
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task<Pet> UpdateAsync(Pet pet) // async Task<Pet>
        {
            _context.Entry(pet).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task DeleteAsync(int petId) // async Task
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
        }
    }
}