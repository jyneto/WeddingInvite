using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.AuthDTO
{
    public class AdminRegisterDTO
    {
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string? UserName { get; set; }

        [Required]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter and one number.")]
        public string? Password { get; set; }
    }
}
