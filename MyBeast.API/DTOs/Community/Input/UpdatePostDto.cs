using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.Community
{
    public class UpdatePostDto
    {
        [MaxLength(200)] // Não obrigatório, permite atualizar só um
        public string? Title { get; set; }

        public string? Content { get; set; } // Permite string vazia ou nula
    }
}