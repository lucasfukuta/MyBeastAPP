using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        // Injetaremos IPetRepository se a exclusão do usuário precisar deletar o Pet
        private readonly IPetRepository _petRepository;

        public UserService(IUserRepository userRepository, IPetRepository petRepository) // Adiciona IPetRepository
        {
            _userRepository = userRepository;
            _petRepository = petRepository;
        }

        // --- MÉTODOS EXISTENTES (AJUSTADOS PARA NÃO RETORNAR HASH) ---

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            // Garante que o hash nunca saia (embora o repositório já deva fazer isso)
            foreach (var user in users) { user.PasswordHash = string.Empty; }
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.PasswordHash = string.Empty; // Limpa o hash
            }
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Este método é mais para uso interno (login/registro).
            // Se exposto diretamente, NUNCA retornar o hash.
            // Por segurança, vamos limpá-lo aqui também por enquanto.
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                user.PasswordHash = string.Empty; // Limpa o hash
            }
            return user;
            // Em um cenário real de login, este serviço receberia a senha,
            // buscaria o usuário COM o hash usando o repositório,
            // compararia os hashes, e retornaria um token ou DTO sem o hash.
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            // Validações básicas (melhorar depois)
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email é obrigatório.");
            if (string.IsNullOrWhiteSpace(user.Username)) throw new ArgumentException("Username é obrigatório.");
            if (string.IsNullOrWhiteSpace(user.PasswordHash)) throw new ArgumentException("Senha é obrigatória."); // Recebe a senha pura aqui

            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null) throw new Exception("Email já cadastrado.");

            // --- HASHING DA SENHA (ESSENCIAL!) ---
            // NUNCA salve a senha em texto plano!
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash); // Exemplo com BCrypt
            // ------------------------------------

            user.CreatedAt = DateTime.UtcNow;
            user.IsModerator = false;
            user.PlanType = user.PlanType ?? "Free"; // Garante valor padrão

            var newUser = await _userRepository.AddAsync(user);
            newUser.PasswordHash = string.Empty; // Limpa o hash antes de retornar
            return newUser;
        }

        // --- NOVOS MÉTODOS ---

        public async Task<User> UpdateUserProfileAsync(int id, string? username, string? email)
        {
            var userToUpdate = await _userRepository.GetByIdAsync(id);
            if (userToUpdate == null) throw new Exception($"Usuário com ID {id} não encontrado.");

            // Validar e Atualizar Username (se fornecido)
            if (!string.IsNullOrWhiteSpace(username))
            {
                if (username.Length > 50) throw new ArgumentException("Username não pode exceder 50 caracteres.");
                userToUpdate.Username = username;
            }

            // Validar e Atualizar Email (se fornecido e diferente)
            if (!string.IsNullOrWhiteSpace(email) && userToUpdate.Email != email)
            {
                // Verificar se o novo email já está em uso por outro usuário
                var existingUserWithEmail = await _userRepository.GetByEmailAsync(email);
                if (existingUserWithEmail != null && existingUserWithEmail.UserId != id)
                {
                    throw new Exception("Este email já está sendo usado por outra conta.");
                }
                if (email.Length > 100) throw new ArgumentException("Email não pode exceder 100 caracteres."); // Adicionar validação de formato de email
                userToUpdate.Email = email;
            }

            // Atualizar no banco
            var updatedUser = await _userRepository.UpdateAsync(userToUpdate);
            updatedUser.PasswordHash = string.Empty; // Limpa o hash
            return updatedUser;
        }

        public async Task DeleteUserAsync(int id)
        {
            var userToDelete = await _userRepository.GetByIdAsync(id);
            if (userToDelete == null) throw new Exception($"Usuário com ID {id} não encontrado.");

            // Lógica adicional:
            // - Deletar Pet associado (se a relação não for Cascade on Delete no DB)
            // var pet = await _petRepository.GetByUserIdAsync(id);
            // if (pet != null) await _petRepository.DeleteAsync(pet.PetId); // Precisaria adicionar DeleteAsync no IPetRepository

            // - Anonimizar posts na comunidade? Ou deletá-los? (Depende da regra de negócio)

            await _userRepository.DeleteAsync(id);
        }
    }
}