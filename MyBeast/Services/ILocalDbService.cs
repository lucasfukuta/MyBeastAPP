using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    internal interface ILocalDbService
    {
        Task<bool> SaveDataAsync<T>(string key, T data);
        Task<T?> GetDataAsync<T>(string key);
        Task<bool> DeleteDataAsync(string key);
        Task<IEnumerable<string>> GetAllKeysAsync();
    }
}
