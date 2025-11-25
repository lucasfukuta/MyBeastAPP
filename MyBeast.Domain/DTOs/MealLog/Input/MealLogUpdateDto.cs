using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyBeast.Domain.DTOs.MealLog.Input;

namespace MyBeast.Domain.DTOs.MealLog.Input
{
    // DTO para receber dados ao atualizar uma refeição
    public class MealLogUpdateDto
    {
        // Campos que podem ser atualizados (todos opcionais no PUT)
        public DateTime? Date { get; set; }

        [MaxLength(50)]
        public string? MealType { get; set; }

        // Lista completa de itens (substitui a lista existente se fornecida)
        [MinLength(0)] // Permite lista vazia para remover todos os itens
        public List<MealItemDto>? Items { get; set; }
    }
}