using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MinimalApi.Domain.DTOs.Enums;

namespace MinimalApi.Domain.Entities
{
    public class Administrator 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = null!;
        [StringLength(10)]
        public Profile Profile { get; set; } = Profile.EDITOR;
    }
}