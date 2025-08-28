namespace WeddingInvite.Api.DTOs.GuestDTO
{
    public class GuestCreateDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsAttending { get; set; }
        public string? Allergies{ get; set; }
    }
}
