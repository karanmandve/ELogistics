using domain.Model.CountryState;
using domain.Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace domain.ModelDto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
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
        public DateTime DateCreated { get; set; }

    }
}
