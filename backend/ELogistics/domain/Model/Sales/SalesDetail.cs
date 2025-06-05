using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace domain.Model.Sales
{
    public class SalesDetail
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Invoice")]
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        [Required, MaxLength(50)]
        public string ProductCode { get; set; }

        [Column(TypeName = "decimal(11,2)")]
        public decimal ProductMRP { get; set; }

        [Column(TypeName = "decimal(11,2)")]
        public decimal ProductRate { get; set; }
        public int CGST { get; set; }
        public int SGST { get; set; }
        public int SaleQuantity { get; set; }
        [Column(TypeName = "decimal(11,2)")]
        public decimal TotalAmount { get; set; }
    }
}
