// using Azure.Storage.Blobs;
// using core.Interface;
// using domain.Model.Sales;
// using domain.ModelDto;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// 
// namespace core.App.Cart.Command
// {
//     public class FakeCheckoutCommand : IRequest<CheckoutResponseDto>
//     {
//         public domain.ModelDto.FakeCheckoutDto CheckoutData { get; set; }
//     }
// 
//     public class FakeCheckoutCommandHandler : IRequestHandler<FakeCheckoutCommand, CheckoutResponseDto>
//     {
//         private readonly IAppDbContext _context;
//         private readonly IInvoiceGenerator _invoiceGenerator;
//         private readonly IEmailService _emailService;
//         private readonly string _connectionString;
//         private readonly string _containerName = "ecomapplication";
//         private readonly string _folderName = "invoices";
// 
//         public FakeCheckoutCommandHandler(IAppDbContext context, IConfiguration configuration, IInvoiceGenerator invoiceGenerator, IEmailService emailService)
//         {
//             _context = context;
//             _connectionString = configuration.GetConnectionString("AzureBlobStorage");
//             _invoiceGenerator = invoiceGenerator;
//             _emailService = emailService;
//         }
// 
//         public async Task<CheckoutResponseDto> Handle(FakeCheckoutCommand request, CancellationToken cancellationToken)
//         {
//             var data = request.CheckoutData;
// 
// 
//             var card = await _context.Set<domain.Model.Cards.Card>()
//                 .FirstOrDefaultAsync(c =>
//                     c.CardNumber == data.CardNumber &&
//                     c.ExpiryDate == data.ExpiryDate &&
//                     c.CVV == data.CVV);
// 
//             if (card == null)
//                 return new CheckoutResponseDto
//                 {
//                     Success = false,
//                     Message = "Invalid card details."
//                 };
// 
// 
//             var cartMaster = await _context.Set<domain.Model.Cart.CartMaster>()
//                 .Include(cm => cm.CartDetails)
//                 .ThenInclude(cd => cd.Product)
//                 .FirstOrDefaultAsync(cm => cm.UserId == data.UserId, cancellationToken);
// 
//             if (cartMaster == null || !cartMaster.CartDetails.Any())
//                 return new CheckoutResponseDto
//                 {
//                     Success = false,
//                     Message = "No items in cart."
//                 };
// 
// 
//             var subtotal = cartMaster.CartDetails.Sum(cd => cd.Quantity * cd.Product.SellingPrice);
// 
// 
//             var salesMaster = new SalesMaster
//             {
//                 InvoiceId = $"ORD-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
//                 UserId = data.UserId,
//                 InvoiceDate = DateTime.UtcNow,
//                 Subtotal = subtotal,
//                 DeliveryAddress = data.DeliveryAddress,
//                 DeliveryZipcode = data.DeliveryZipcode,
//                 DeliveryStateId = data.DeliveryStateId,
//                 DeliveryCountryId = data.DeliveryCountryId,
//             };
// 
//             await _context.Set<domain.Model.Sales.SalesMaster>().AddAsync(salesMaster, cancellationToken);
//             await _context.SaveChangesAsync(cancellationToken);
// 
// 
//             foreach (var cartDetail in cartMaster.CartDetails)
//             {
//                 var salesDetail = new SalesDetail
//                 {
//                     InvoiceId = salesMaster.Id,
//                     ProductId = cartDetail.ProductId,
//                     ProductCode = cartDetail.Product.ProductCode,
//                     SaleQuantity = cartDetail.Quantity,
//                     SellingPrice = cartDetail.Product.SellingPrice
//                 };
// 
//                 await _context.Set<domain.Model.Sales.SalesDetail>().AddAsync(salesDetail, cancellationToken);
// 
// 
//                 //cartDetail.Product.Stock -= cartDetail.Quantity;
//                 _context.Set<domain.Model.Products.Product>().Update(cartDetail.Product);
//             }
// 
// 
//             _context.Set<domain.Model.Cart.CartDetail>().RemoveRange(cartMaster.CartDetails);
//             _context.Set<domain.Model.Cart.CartMaster>().Remove(cartMaster);
// 
// 
//             await _context.SaveChangesAsync(cancellationToken);
// 
// 
//             var invoiceStream = await _invoiceGenerator.GenerateInvoice(salesMaster.Id, subtotal, cartMaster.CartDetails.ToList());
// 
// 
//             var blobServiceClient = new BlobServiceClient(_connectionString);
//             var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
//             var blobClient = blobContainerClient.GetBlobClient($"{_folderName}/{salesMaster.InvoiceId}.pdf");
// 
//             await blobClient.UploadAsync(invoiceStream, true);
// 
// 
//             var invoiceUrl = blobClient.Uri.ToString();
// 
//             salesMaster.InvoicePdfLink = invoiceUrl;
//             _context.Set<domain.Model.Sales.SalesMaster>().Update(salesMaster);
// 
//             var userData = await _context.Set<domain.Model.Users.User>().FirstOrDefaultAsync(u => u.Id == data.UserId);
// 
//             await _context.SaveChangesAsync(cancellationToken);
// 
//             await _emailService.SendEmailAsync(
//                     userData.Email,
//                     "Order Confirmation - EComApplication",
//                     $"<html><body style='font-family: Arial, sans-serif;'>" +
//                     $"<table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 20px;'>" +
//                     $"  <tr>" +
//                     $"    <td align='center' style='background-color: #2E3B4E; padding: 20px; color: white; font-size: 24px; font-weight: bold;">" +
//                     $"      <h1 style='margin: 0; color: white;'>EComApplication</h1>" +
//                     $"    </td>" +
//                     $"  </tr>" +
//                     $"  <tr>" +
//                     $"    <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);">" +
//                     $"      <p style='font-size: 18px;'>Dear {userData.FirstName},</p>" +
//                     $"      <p style='font-size: 16px;'>Congratulations! Your order has been successfully placed on <strong>EComApplication</strong>.</p>" +
//                     $"      <p style='font-size: 16px;'><strong>Order ID:</strong> {salesMaster.InvoiceId}</p>" +
//                     $"      <p style='font-size: 16px;'><strong>Total Amount:</strong> ₹{subtotal}</p>" +
//                     $"      <p style='font-size: 16px;'>You can download your invoice from the following link:</p>" +
//                     $"      <p style='font-size: 16px;'><a href='{invoiceUrl}' target='_blank' style='color: #2E3B4E;'>Download Invoice</a></p>" +
//                     $"      <p style='font-size: 16px;'>Thank you for choosing <strong>EComApplication</strong>. We look forward to serving you again.</p>" +
//                     $"      <p style='font-size: 16px;'>Best regards,<br>The EComApplication Team</p>" +
//                     $"    </td>" +
//                     $"  </tr>" +
//                     $"  <tr>" +
//                     $"    <td align='center' style='background-color: #2E3B4E; padding: 20px; color: white; font-size: 14px;">" +
//                     $"      <p>© {DateTime.Now.Year} EComApplication. All rights reserved.</p>" +
//                     $"    </td>" +
//                     $"  </tr>" +
//                     $"</table>" +
//                     $"</body></html>"
//                 );
// 
//             return new CheckoutResponseDto
//             {
//                 Success = true,
//                 Message = "Order placed successfully.",
//                 InvoiceId = salesMaster.InvoiceId,
//                 InvoiceUrl = invoiceUrl
//             };
//         }
//     }
// }
