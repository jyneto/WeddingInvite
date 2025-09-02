using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class BookingGetDTO
    { 
        public int Id { get; set; }
        [Required]
        public int TableId { get; set; }
        [Required]
        public int GuestId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        [Range(1, 4 , ErrorMessage = "Party size must be between 1 and 4.")]
        public int PartySize { get; set; }
    }
}
