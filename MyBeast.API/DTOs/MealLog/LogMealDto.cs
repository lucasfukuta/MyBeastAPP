using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para validações

namespace MyBeast.API.Dtos.MealLog
{
    // DTO para receber dados ao registrar uma refeição
    public class LogMealDto
    {
        [Required]
        public int UserId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string MealType { get; set; } = string.Empty; // Ex: 'Breakfast', 'Lunch'

        [Required]
        [MinLength(1, ErrorMessage = "Pelo menos um item alimentar é necessário.")]
        public List<MealItemDto> Items { get; set; } = new List<MealItemDto>();
    }

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