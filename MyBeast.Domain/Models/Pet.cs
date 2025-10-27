using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Models
{
    [Table("Pets")]
    public class Pet
    {
        [Key]
        public int PetId { get; set; } 

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } 
        public int EvolutionLevel { get; set; } 
        public int Health { get; set; } 
        public int Energy { get; set; } 
        public int Hunger { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } 
    }
}