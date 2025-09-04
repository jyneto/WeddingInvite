using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.GuestDTO
{
    public class GuestUpdateDTO
    {
        [Required(ErrorMessage = "Fullname is required")]
        [MaxLength(100, ErrorMessage = "Fullname can't be longer than 100 characters")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        public bool IsAttending { get; set; }
        [MaxLength(250, ErrorMessage = "Allergies can't be longer than 250 characters")]
        public string? Allergies{ get; set; }
    }
}
