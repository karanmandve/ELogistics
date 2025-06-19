using Domain.Enums;

namespace domain.ModelDtos
{
    public class CartProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductMRP { get; set; }
        public decimal ProductRate { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal TotalPrice { get; set; }
        public GstSlab SGST { get; set; }
        public GstSlab CGST { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
    }
}
