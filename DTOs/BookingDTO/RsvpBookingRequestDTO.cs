namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class RsvpBookingRequestDTO
    {
        public string FullName { get; set; } = string .Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone {  get; set; }

        public string? Allergies { get; set; }

        public bool IsAttending { get; set; }

        public int TableId { get; set; }
        public DateTime StartTime { get; set; }
        public int PartySize { get; set; }
    }
}
