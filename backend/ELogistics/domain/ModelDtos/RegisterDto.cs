using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using domain.Model.States;
using domain.Model.Users;

namespace domain.ModelDtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int UserTypeId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }
        
        public int StateId { get; set; }

        public string City { get; set; }
        
        public string Zip { get; set; }
        
        public string Line1 { get; set; }
        
        public string? Line2 { get; set; }
    }
}
