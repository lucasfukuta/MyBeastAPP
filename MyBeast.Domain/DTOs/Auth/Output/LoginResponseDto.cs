using MyBeast.Domain.DTOs.User.Output;

namespace MyBeast.Domain.DTOs.Auth.Output
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; } = null!; // Retorna dados do usuário logado
    }
}
