using MyBeast.Domain.DTOs.MealLog.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBeast.Domain.DTOs.MealLog.Input
{
    public class LogMealDto
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string MealType { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Pelo menos um item alimentar é necessário.")]
        public List<MealItemDto> Items { get; set; } = new List<MealItemDto>();
    }
}