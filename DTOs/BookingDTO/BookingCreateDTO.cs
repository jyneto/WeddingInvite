using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class BookingCreateDTO
    {
        [Required]
        public int TableId { get; set; }
        [Required]
        public int GuestId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Party size is required.")]
        [Range(1, 4, ErrorMessage = "Party size must be between 1 and 4.")]
        public int PartySize { get; set; }
    }
}
