using domain.Model.Sales;
using domain.Model.Users;

namespace core.Interface
{
   public interface IInvoiceGenerator
   {
       Task<MemoryStream> GenerateInvoice(Guid invoiceId, List<SalesDetail> salesDetail, Customer customerData, Distributor distributorData);
   }
}
