using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Models.DTOs.Diet
{
    public class MealItemResponseDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }
    }
}
