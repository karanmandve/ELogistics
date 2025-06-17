namespace domain.ModelDtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserTypeId { get; set; }
        public Guid? DistributorId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int StateId { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string? GSTNumber { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
    }
}