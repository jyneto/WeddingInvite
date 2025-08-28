using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(120)]
        public string? Name { get; set; }
        [MaxLength(400)]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public bool IsPopular { get; set; }


        //public string? ImageUrl { get; set; }


    }
}
