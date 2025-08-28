namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class BookingCreateDTO
    {
        public int TableId { get; set; }
        public int GuestId { get; set; }
        public DateTime StartTime { get; set; }
        public int PartySize { get; set; }
    }
}
