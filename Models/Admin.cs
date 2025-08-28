using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.Models
{
    public class Admin
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string? UserName { get; set; }

        [Required]
        public string? PasswordHash { get; set; }
    }
}
