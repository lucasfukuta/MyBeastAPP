using System.Net.Http.Json;
// Importe os DTOs que agora estão no DOMAIN
using MyBeast.Domain.DTOs.Auth.Input;
using MyBeast.Domain.DTOs.Auth.Output;
using MyBeast.Domain.DTOs.User.Input;
using MyBeast.Domain.DTOs.User.Output; // Para UserDto (resposta do registro)

namespace MyBeast.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        // Opcional: Se quiser salvar dados do usuário no banco local
        // private readonly ILocalDbService _localDbService; 

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // 1. Prepara o DTO de Login (LoginRequestDto)
                // Atenção: Seu Controller pede 'Email', mas a variável chama 'Username'.
                // Se o usuário pode logar com ambos, o backend deve tratar.
                // Se for só Email, mude a propriedade abaixo para Email = ...
                var loginDto = new LoginRequestDto
                {
                    Email = username, // Assumindo que o login é via Email conforme seu Controller
                    Password = password
                };

                // 2. Chama o endpoint /api/Auth/login
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    // 3. Lê a resposta (LoginResponseDto)
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        // 4. Salva o Token para uso futuro (SecureStorage)
                        await SecureStorage.SetAsync("auth_token", result.Token);

                        // (Opcional) Salvar dados do usuário logado
                        // await _localDbService.SaveDataAsync("user_data", result.User);

                        return true;
                    }
                }
                else
                {
                    // (Opcional) Ler mensagem de erro da API para debug
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Falha no Login: {errorMsg}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de Conexão (Login): {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            try
            {
                // 1. Prepara o DTO de Registro (UserRegisterDto)
                var registerDto = new UserRegisterDto
                {
                    Username = username,
                    Email = email,
                    Password = password,
                    PlanType = "Free" // Define um padrão ou receba por parâmetro
                };

                // 2. CORREÇÃO: Chama o endpoint /api/Users/register (Baseado no seu UsersController)
                var response = await _httpClient.PostAsJsonAsync("api/Users/register", registerDto);

                // Se criou (201 Created), deu certo!
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de Conexão (Registro): {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            SecureStorage.Remove("auth_token");
        }

        public Task LogoutAsync()
        {
            Logout();
            return Task.CompletedTask;
        }

        // Métodos legados
        public bool Login(string u, string p) => throw new NotImplementedException();
        public bool Register(string u, string p) => throw new NotImplementedException();
        public Task<bool> ChangePasswordAsync(string u, string o, string n) => throw new NotImplementedException();
        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync("auth_token");
        }
    }
}