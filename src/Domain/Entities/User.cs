using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required(ErrorMessage = "First name cannot be empty.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name cannot be empty.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email cannot be empty.")]
        [EmailAddress]
        public required string Email { get; set; }

        [PasswordPropertyText]
        public required string Password { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required UserRole Role { get; set; }
    }
}
