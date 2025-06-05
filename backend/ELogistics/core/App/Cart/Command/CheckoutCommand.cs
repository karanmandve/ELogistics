using core.API_Response;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using core.Interface;
using domain.Model.Sales;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using domain.Model.Cart;
using domain.Model.Users;
using Domain.Enums;

namespace core.App.Cart.Command
{
    public class CheckoutCommand : IRequest<AppResponse<object>>
    {
        public Guid CustomerId { get; set; }
    }
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IInvoiceGenerator _invoiceGenerator;
        private readonly IEmailService _emailService;
        private readonly string _connectionString;
        private readonly string _containerName = "elogistics";
        private readonly string _folderName = "invoices";
        public CheckoutCommandHandler(IAppDbContext context, IConfiguration configuration, IInvoiceGenerator invoiceGenerator, IEmailService emailService)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _invoiceGenerator = invoiceGenerator;
            _emailService = emailService;
        }
        public async Task<AppResponse<object>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cartMaster = await _context.Set<CartMaster>()
                .Include(cm => cm.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(cm => cm.CustomerId == request.CustomerId, cancellationToken);

            if (cartMaster == null)
                return AppResponse.Fail<object>(message: "No items in cart.", statusCode: HttpStatusCodes.BadRequest);

            // generating invoice Id
            var invoiceData = new Invoice
            {
                CustomerId = request.CustomerId,
                InvoiceDate = DateTime.Now,
            };

            await _context.Set<Invoice>().AddAsync(invoiceData, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // saving data to sales details
            foreach (var cartDetail in cartMaster.CartDetails)
            {
                var salesDetail = new SalesDetail
                {
                    InvoiceId = invoiceData.Id,
                    ProductName = cartDetail.Product.ProductName,
                    ProductCode = cartDetail.Product.ProductCode,
                    ProductMRP = cartDetail.Product.ProductMRP,
                    ProductRate = cartDetail.Product.ProductRate,
                    CGST = (int)GstSlab.GST_9,
                    SGST = (int)GstSlab.GST_9,
                    SaleQuantity = cartDetail.Quantity,
                    TotalAmount = cartDetail.Quantity * cartDetail.Product.ProductRate
                };
                await _context.Set<SalesDetail>().AddAsync(salesDetail, cancellationToken);
            }

            // remove all the data present in cart
            _context.Set<CartDetail>().RemoveRange(cartMaster.CartDetails);
            _context.Set<CartMaster>().Remove(cartMaster);
            await _context.SaveChangesAsync(cancellationToken);

            // send data to invoice generator to generate invoice
            var salesData = await _context.Set<SalesDetail>().Where(sd => sd.InvoiceId == invoiceData.Id).ToListAsync();
            var customerData = await _context.Set<Customer>().Include(c => c.State).FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);
            var distributorData = await _context.Set<Distributor>().Include(c => c.State).FirstOrDefaultAsync(d => d.Id == customerData.DistributorId, cancellationToken);
            var invoiceStream = await _invoiceGenerator.GenerateInvoice(invoiceData.Id, salesData, customerData, distributorData);

            // saving invoide in blob storage
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient($"{_folderName}/{invoiceData.Id}.pdf");

            await blobClient.UploadAsync(invoiceStream, true);

            // saving  invoice url in invoice data
            var invoiceUrl = blobClient.Uri.ToString();
            invoiceData.InvoicePdfLink = invoiceUrl;

            _context.Set<Invoice>().Update(invoiceData);

            var CustomerData = await _context.Set<Customer>().FindAsync(request.CustomerId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var subTotal = salesData.Sum(sd => sd.TotalAmount);
            var totalCGST = salesData.Sum(sd => sd.TotalAmount * ((int)sd.CGST) / 100m);
            var totalSGST = salesData.Sum(sd => sd.TotalAmount * ((int)sd.SGST) / 100m);
            var grandTotal = subTotal + totalCGST + totalSGST;

            // Build email body
            var emailBody = $@"
                            <html>
                            <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; margin: 0; padding: 0;'>
                                <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                                    <tr>
                                        <td align='center' style='background-color: #004085; padding: 30px;'>
                                            <h1 style='margin: 0; color: #ffffff; font-size: 28px;'>ELogistics</h1>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                                            <p style='font-size: 18px; color: #0056b3; margin-bottom: 12px;'>Dear {customerData.FirstName},</p>
                                            <p style='font-size: 16px; color: #333333; line-height: 1.5; margin-bottom: 20px;'>Thank you for your order! Your purchase has been confirmed with the following details:</p>
                                            <table width='100%' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>
                                                <tr style='background-color: #f1f1f1;'>
                                                    <th align='left'>Item</th>
                                                    <th align='center'>Qty</th>
                                                    <th align='right'>Rate (₹)</th>
                                                    <th align='right'>CGST</th>
                                                    <th align='right'>SGST</th>
                                                    <th align='right'>Amount (₹)</th>
                                                </tr>
                                                {string.Join("", salesData.Select(sd => $"<tr>" +
                                                    $"<td>{sd.ProductName}</td>" +
                                                    $"<td align='center'>{sd.SaleQuantity}</td>" +
                                                    $"<td align='right'>{sd.ProductRate:F2}</td>" +
                                                    $"<td align='right'>{(int)sd.CGST}%</td>" +
                                                    $"<td align='right'>{(int)sd.SGST}%</td>" +
                                                    $"<td align='right'>{sd.TotalAmount:F2}</td>" +
                                                    $"</tr>"))}
                                                <tr>
                                                    <td colspan='5' align='right'><strong>Subtotal:</strong></td>
                                                    <td align='right'>₹{subTotal:F2}</td>
                                                </tr>
                                                <tr>
                                                    <td colspan='5' align='right'><strong>Total CGST:</strong></td>
                                                    <td align='right'>₹{totalCGST:F2}</td>
                                                </tr>
                                                <tr>
                                                    <td colspan='5' align='right'><strong>Total SGST:</strong></td>
                                                    <td align='right'>₹{totalSGST:F2}</td>
                                                </tr>
                                                <tr style='background-color: #f1f1f1;'>
                                                    <td colspan='5' align='right'><strong>Grand Total:</strong></td>
                                                    <td align='right'><strong>₹{grandTotal:F2}</strong></td>
                                                </tr>
                                            </table>
                                            <p style='font-size: 16px;'><a href='{invoiceUrl}' target='_blank' style='color: #004085; text-decoration: none;'>Download your invoice</a></p>
                                            <p style='font-size: 16px; color: #333333;'>We appreciate your business and hope to serve you again soon.</p>
                                            <p style='font-size: 16px; color: #0056b3; margin: 0;'>Best regards,<br/>The ELogistics Team</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align='center' style='background-color: #004085; padding: 20px;'>
                                            <p style='margin: 0; font-size: 14px; color: #dddddd;'>© {DateTime.Now.Year} ELogistics. All rights reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";

            await _emailService.SendEmailAsync(CustomerData.Email, "Order Confirmation - ELogistics", emailBody);

            var response = new
            {
                InvoiceUrl = invoiceUrl
            };

            return AppResponse.Success((object)response, "Order placed successfully.", HttpStatusCodes.OK);
        }
    }
}
