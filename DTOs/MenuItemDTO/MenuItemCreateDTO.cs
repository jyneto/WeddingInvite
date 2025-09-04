using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.MenuItemDTO
{
    public class MenuItemCreateDTO
    {
        [Required]
        [MaxLength(120)]
        public string? Name { get; set; }
        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        public decimal Price { get; set; }
        [MaxLength(400)]
        public string? Description { get; set; }
        public bool IsPopular { get; set; }
        //public string? ImageUrl { get; set; }
    }
}
