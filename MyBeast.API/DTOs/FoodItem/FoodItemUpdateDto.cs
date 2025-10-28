using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.Dtos.FoodItem
{
    public class FoodItemUpdateDto
    {
        // Campos que podem ser atualizados
        [MaxLength(100)]
        public string? Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Calories { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Protein { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Carbs { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Fat { get; set; }
    }
}