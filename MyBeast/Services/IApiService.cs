using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    internal interface IApiService
    {
        Task<Achievement> GetAchievementAsync(string url);
        Task<IEnumerable<Achievement>> GetAchievementsAsync(string url);
        Task<bool> PostAchievementAsync(string url, Achievement achievement);
        Task<bool> PutAchievementAsync(string url, Achievement achievement);
        Task<bool> DeleteAchievementAsync(string url);
    }
}
