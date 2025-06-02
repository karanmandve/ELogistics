using domain.Model.CountryState;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace domain.Model.User
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        [ForeignKey("State")]
        public int StateId { get; set; }
        public State State { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public string Pincode { get; set; }

        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }

        public string? Qualification { get; set; }

        [ForeignKey("Specialisation")]
        public int? SpecialisationId { get; set; }
        public Specialisation Specialisation { get; set; }

        public string? RegistrationNumber { get; set; }
        public float? VisitingCharge { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }


    }
}
