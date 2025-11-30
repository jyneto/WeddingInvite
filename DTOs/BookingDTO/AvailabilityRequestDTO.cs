using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class AvailabilityRequestDTO
    {
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        [Range(1, 2, ErrorMessage = "Party size must be between 1 and 2.")]
        public int PartySize { get; set; }
    }
}
