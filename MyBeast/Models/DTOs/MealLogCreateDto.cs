namespace MyBeast.Domain.DTOs.MealLog.Input
{
    public class MealLogCreateDto
    {
        public string Name { get; set; } 
        public DateTime Date { get; set; } 
        public List<MealLogItemDto> Items { get; set; } = new();
    }

    public class MealLogItemDto
    {
        public int FoodId { get; set; }
        public double Quantity { get; set; } 
    }
}