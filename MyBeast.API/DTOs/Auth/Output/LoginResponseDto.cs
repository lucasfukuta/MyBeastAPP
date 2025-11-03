using MyBeast.API.DTOs.User.Output;

namespace MyBeast.API.DTOs.Auth.Output
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; } = null!; // Retorna dados do usuário logado
    }
}
