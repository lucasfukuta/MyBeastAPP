using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Entities;
using System.Collections.Generic;

namespace MyBeast.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        // Liste aqui as tabelas que você precisa acessar
        DbSet<FoodItem> FoodItems { get; }
        DbSet<Exercise> Exercises { get; }
        // Adicione outros DbSets conforme precisar (Users, etc)

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}