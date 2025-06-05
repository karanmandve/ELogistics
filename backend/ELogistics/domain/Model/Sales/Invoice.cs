using domain.Model.Products;
using domain.Model.States;
using domain.Model.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace domain.Model.Sales
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime InvoiceDate { get; set; }
        public string InvoicePdfLink { get; set; }
    }
}
