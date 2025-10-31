using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.Pets.Input
{
    // DTO para atualização geral do Pet
    public class PetUpdateDto
    {
        [MaxLength(50)]
        public string? Name { get; set; } // Opcional, só atualiza se enviado

        // Adicionar outros campos atualizáveis aqui (ex: Skins, etc.)
    }
}