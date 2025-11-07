using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de SetLog
    // Nota: Geralmente manipulado através da WorkoutSession
    public interface ISetLogRepository
    {
        Task<IEnumerable<SetLog>> GetBySessionIdAsync(int sessionId);
        Task AddRangeAsync(IEnumerable<SetLog> setLogs); // Para adicionar vários sets de uma vez
        // Update/Delete de SetLogs pode ser complexo, faremos se necessário
    }
}