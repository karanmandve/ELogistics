using domain.Model.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace domain.Model.Cart
{
    public class CartDetail
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("CartMaster")]
        public Guid CartId { get; set; }
        public CartMaster CartMaster { get; set; }

        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
