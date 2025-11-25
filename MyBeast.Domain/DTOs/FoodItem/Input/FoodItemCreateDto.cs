using System.ComponentModel.DataAnnotations;

namespace MyBeast.Domain.DTOs.FoodItem.Input
{
    public class FoodItemCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Calories { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Protein { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Carbs { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Fat { get; set; }

        // A propriedade 'UserId' foi REMOVIDA
    }
}