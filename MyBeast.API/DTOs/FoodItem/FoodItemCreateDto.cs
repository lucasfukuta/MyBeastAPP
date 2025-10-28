using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.Dtos.FoodItem
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

        [Required] // UserId será pego do usuário autenticado no Controller
        public int UserId { get; set; } // Temporário: Remover quando tiver Auth
    }
}