namespace WeddingInvite.Api.DTOs.BookingDTO
{
    public class BookingGetDTO
    {
        public int Id { get; set; }
        public int TableId { get; set; }

        public int GuestId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PartySize { get; set; }
    }
}
