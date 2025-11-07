using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    internal interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<bool> RegisterAsync(string username, string password, string email);
        Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
    }
}
