using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Models.DTOs.Diet
{
    public class MealLogDto
    {
        public int MealLogId { get; set; }
        public string MealType { get; set; }
        public DateTime LogDate { get; set; }
        public List<MealItemResponseDto> Items { get; set; } = new List<MealItemResponseDto>();
        public decimal TotalCalories { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalFat { get; set; }
    }
}
