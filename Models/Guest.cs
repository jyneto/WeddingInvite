using System.ComponentModel.DataAnnotations;

namespace WeddingInvite.Api.Models
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public bool IsAttending { get; set; }
        [MaxLength(250)]
        public string? Allergies { get; set; }

        public int? TableId { get; set; }
        public Table? Table { get; set; }
    }
}
