using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(255)]
        public string Brand { get; set; } = null!;
        [Required]
        public int Year { get; set; }
    }
}