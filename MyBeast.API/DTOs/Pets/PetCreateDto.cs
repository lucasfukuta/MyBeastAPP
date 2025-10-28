namespace MyBeast.API.Dtos.Pet // Verifique/Ajuste o namespace
{
    public class PetCreateDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}