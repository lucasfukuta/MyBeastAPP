using System.Text.Json;

namespace MyBeast.Services
{
    public class LocalDbService : ILocalDbService
    {
        // Salva qualquer tipo de objeto (T) transformando em texto (JSON)
        public Task<bool> SaveDataAsync<T>(string key, T data)
        {
            try
            {
                if (data == null) return Task.FromResult(false);

                string jsonString = JsonSerializer.Serialize(data);

                // Preferences é nativo do MAUI. Salva dados leves no celular.
                Preferences.Default.Set(key, jsonString);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar localmente: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        // Recupera o dado e transforma de volta em objeto
        public Task<T?> GetDataAsync<T>(string key)
        {
            try
            {
                if (!Preferences.Default.ContainsKey(key))
                    return Task.FromResult<T?>(default);

                string jsonString = Preferences.Default.Get(key, string.Empty);

                if (string.IsNullOrEmpty(jsonString))
                    return Task.FromResult<T?>(default);

                var data = JsonSerializer.Deserialize<T>(jsonString);
                return Task.FromResult(data);
            }
            catch
            {
                return Task.FromResult<T?>(default);
            }
        }

        public Task<bool> DeleteDataAsync(string key)
        {
            Preferences.Default.Remove(key);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<string>> GetAllKeysAsync()
        {
            // Preferences não tem um método nativo fácil para listar todas as chaves
            // de forma genérica, então retornamos vazio ou implementamos lógica customizada.
            // Para este uso básico, podemos deixar vazio ou lançar exceção.
            return Task.FromResult(Enumerable.Empty<string>());
        }
    }
}