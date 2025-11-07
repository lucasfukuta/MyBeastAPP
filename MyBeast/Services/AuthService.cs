using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    internal class AuthService: IAuthService
    {
        public Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool Login(string username, string password)
        {
            // Implementação fictícia de autenticação
            if (username == "admin" && password == "password")
            {
                return true;
            }
            return false;
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            // Implementação fictícia de logout
            Console.WriteLine("Usuário desconectado.");
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public bool Register(string username, string password)
        {
            // Implementação fictícia de registro
            Console.WriteLine($"Usuário {username} registrado com sucesso.");
            return true;
        }

        public Task<bool> RegisterAsync(string username, string password, string email)
        {
            throw new NotImplementedException();
        }
    }
}
