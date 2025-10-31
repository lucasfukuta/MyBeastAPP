using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.Dtos.MealLog.Input
{
    // DTO para representar um item alimentar dentro da refeição
    public class MealItemDto
    {
        [Required]
        public int FoodId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public decimal Quantity { get; set; }
    }
}