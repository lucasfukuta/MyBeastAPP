using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.Dtos.Pet
{
    // DTO para atualização geral do Pet
    public class PetUpdateDto
    {
        [MaxLength(50)]
        public string? Name { get; set; } // Opcional, só atualiza se enviado

        // Adicionar outros campos atualizáveis aqui (ex: Skins, etc.)
    }
}