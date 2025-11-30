using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WeddingInvite.Api.DTOs.GuestDTO
{
    public class GuestCreateDTO : IValidatableObject
    {
        [JsonPropertyName("fullName")]
        [Required(ErrorMessage = "Fullname is required")]
        [MaxLength(100, ErrorMessage = "Fullname can't be longer than 100 characters")]
        public string? FullName { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [JsonPropertyName("isAttending")]
        public bool IsAttending { get; set; }

        [JsonPropertyName("allergies")]
        [MaxLength(250, ErrorMessage = "Allergies can't be longer than 250 characters")]
        public string? Allergies { get; set; }

        [JsonPropertyName("tableId")]
        public int? TableId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (IsAttending)
            {
                if (TableId is null || TableId <= 0)
                {
                    yield return new ValidationResult(
                        "TableId is required and must be greater than 0 when IsAttending is true.",
                        new[] { nameof(TableId) });
                }
            }
            else
            {
                if (TableId is not null)
                {
                    yield return new ValidationResult(
                        "TableId must be null when IsAttending is false.",
                        new[] { nameof(TableId) });
                }
            }
        }


    }

}



    

    

