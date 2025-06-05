using domain.Model.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace domain.Model.Cart
{
    public class CartMaster
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}
