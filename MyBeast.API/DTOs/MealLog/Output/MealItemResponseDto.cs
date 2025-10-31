namespace MyBeast.API.DTOs.MealLog.Output
{
    // DTO para retornar dados de um MealLogItem (incluindo nome do alimento)
    public class MealItemResponseDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty; // Nome do Alimento
        public decimal Quantity { get; set; }
        // Incluir macros calculados (opcional)?
        // public decimal CalculatedCalories { get; set; }
        // public decimal CalculatedProtein { get; set; }
        // public decimal CalculatedCarbs { get; set; }
        // public decimal CalculatedFat { get; set; }
    }
}