using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyBeast.API.Dtos.MealLog.Input;

namespace MyBeast.API.DTOs.MealLog.Input
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
}