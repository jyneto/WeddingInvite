using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class AvailabilityRequestDTO
    {
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        [Range(1, 4, ErrorMessage = "Party size must be between 1 and 4.")]
        public int PartySize { get; set; }
    }
}
