using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using domain.Model.States;

namespace domain.Model.Users
{
    public class Distributor
    {
        [Key]
        public Guid Id { get; set; }
        
        [ForeignKey("UserType")]
        [Required]
        public int UserTypeId { get; set; }
        public UserType? UserType { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, MaxLength(200), EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [MaxLength(11), Phone]
        public string PhoneNumber { get; set; }

        [ForeignKey("State")]
        public int StateId { get; set; }
        public State State { get; set; }

        public string City { get; set; }

        [MaxLength(10)]
        public string Zip { get; set; }

        [MaxLength(200)]
        public string Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [MaxLength(20)]
        public string? GSTNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
} 