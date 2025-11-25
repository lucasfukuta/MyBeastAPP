namespace MyBeast.Domain.DTOs.FoodItem.Output // Verifique o namespace
{
    // DTO para retornar dados de um FoodItem
    public class FoodItemDto
    {
        public int FoodId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }
        public bool IsCustom { get; set; }
        public int? UserId { get; set; } // Incluir UserId para identificar o criador (se custom)

        // Não incluir propriedades de navegação como User ou MealLogItems
    }
}