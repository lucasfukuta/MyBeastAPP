using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; // Substituindo System.Data.SqlClient por Microsoft.Data.SqlClient
using System.Threading.Tasks;
using MyBeast.Services;
using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    public class LocalDbService: ILocalDbService
    {
        private readonly string _connectionString;

        public LocalDbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, Func<IDataReader, T> map)
        {
            var results = new List<T>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }

        public async Task<int> ExecuteAsync(string query, Dictionary<string, object>? parameters = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return await command.ExecuteNonQueryAsync();
        }

        public Task<bool> SaveDataAsync<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetDataAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDataAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetAllKeysAsync()
        {
            throw new NotImplementedException();
        }
    }
}
