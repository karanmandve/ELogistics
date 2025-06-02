using Microsoft.AspNetCore.Http;

namespace domain.ModelDto
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public IFormFile File { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Pincode { get; set; }
        public int UserTypeId { get; set; }

        public string? Qualification { get; set; }
        public int? SpecialisationId { get; set; }
        public string? RegistrationNumber { get; set; }
        public float? VisitingCharge { get; set; }
    }
}
