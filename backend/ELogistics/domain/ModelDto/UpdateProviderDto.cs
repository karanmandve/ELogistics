using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.ModelDto
{
    public class UpdateProviderDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Pincode { get; set; }
        public string Qualification { get; set; }
        public int SpecialisationId { get; set; }
        public float VisitingCharge { get; set; }
    }
}
