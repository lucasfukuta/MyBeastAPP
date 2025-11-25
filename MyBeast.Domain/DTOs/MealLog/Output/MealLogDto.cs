using System;
using System.Collections.Generic;

namespace MyBeast.Domain.DTOs.MealLog.Output
{
    //Retorna a refeição e a lista completa
    // DTO para retornar dados de um MealLog (incluindo itens formatados)
    public class MealLogDto
    {
        public int MealLogId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string MealType { get; set; } = string.Empty;
        public List<MealItemResponseDto> Items { get; set; } = new List<MealItemResponseDto>(); // Lista de itens detalhados

        // Adicionar totais calculados (opcional)?
        // public decimal TotalCalories { get; set; }
        // public decimal TotalProtein { get; set; }
        // public decimal TotalCarbs { get; set; }
        // public decimal TotalFat { get; set; }
    }
}