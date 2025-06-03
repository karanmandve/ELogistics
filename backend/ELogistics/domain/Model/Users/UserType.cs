using System.ComponentModel.DataAnnotations;

namespace domain.Model.Users
{
    public class UserType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
