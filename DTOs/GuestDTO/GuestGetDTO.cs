namespace WeddingInvite.Api.DTOs.GuestDTO
{
    public class GuestGetDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsAttending { get; set; }
        public string? Allergies { get; set; }
        public int? TableNumber { get; set; }
        public int? TableId { get; set; }
    }
}
