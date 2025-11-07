using Microsoft.Extensions.Configuration; // Para IConfiguration
using Microsoft.IdentityModel.Tokens;     // Para SymmetricSecurityKey, SigningCredentials
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; // Para JwtSecurityToken, JwtSecurityTokenHandler
using System.Security.Claims;          // Para Claims
using System.Text;                     // Para Encoding
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(string Token, DateTime Expiration, User User)> LoginAsync(string email, string password)
        {
            // 1. Encontrar o usuário COM O HASH
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("Email ou senha inválidos.");
            }

            // 2. Verificar a senha
            bool isPasswordValid;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                // Captura erro caso o hash seja inválido (ex: usuário do DataImporter sem hash)
                throw new Exception("Erro de autenticação.", ex);
            }

            if (!isPasswordValid)
            {
                throw new Exception("Email ou senha inválidos.");
            }

            // 3. Gerar o Token
            var (tokenString, expiration) = GenerateJwtToken(user);

            // 4. Retornar a tupla com o MODELO User (sem DTO)
            return (tokenString, expiration, user);
        }

        // --- Método Privado para Gerar o Token ---
        private (string, DateTime) GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new Exception("Chave secreta JWT não configurada no appsettings.json");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(8); // Token válido por 8 horas

            // Claims (Informações que vão dentro do token)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // ID do Usuário (Subject)
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username), // Exemplo de claim customizada
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único do token
                new Claim(ClaimTypes.Role, user.IsModerator ? "Admin" : "User") // Role
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return (tokenHandler.WriteToken(token), expiration);
        }
    }
}