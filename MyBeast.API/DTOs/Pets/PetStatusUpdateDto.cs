namespace MyBeast.API.Dtos.Pet // Verifique/Ajuste o namespace
{
    public class PetStatusUpdateDto
    {
        public int Health { get; set; }
        public int Energy { get; set; }
        public int Hunger { get; set; }
        public string Status { get; set; } = string.Empty; // Valor padrão
    }
}