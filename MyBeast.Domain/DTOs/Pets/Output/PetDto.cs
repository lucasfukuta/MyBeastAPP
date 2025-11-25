namespace MyBeast.Domain.DTOs.Pets.Output // Verifique o namespace
{
    // DTO para retornar dados do Pet
    public class PetDto
    {
        public int PetId { get; set; }
        public int UserId { get; set; } // Incluir UserId pode ser útil no frontend
        public string Name { get; set; } = string.Empty;
        public int EvolutionLevel { get; set; }
        public int Health { get; set; }
        public int Energy { get; set; }
        public int Hunger { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Experience { get; set; }
        public int XpToNextLevel { get; set; }

        // Não incluir a propriedade de navegação 'User'
    }
}