using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Models.DTOs.Diet
{
    public class FoodItemDto
    {
        public int FoodItemId { get; set; }
        public string Name { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbohydrates { get; set; }
        public decimal Fat { get; set; }
    }
}
