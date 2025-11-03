using MyBeast.API.DTOs.MealLog.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.MealLog.Input
{
    public class LogMealDto
    {
        // UserId FOI REMOVIDO. O servidor pegará do token.
        // [Required]
        // public int UserId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string MealType { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Pelo menos um item alimentar é necessário.")]
        public List<MealItemDto> Items { get; set; } = new List<MealItemDto>();
    }
}