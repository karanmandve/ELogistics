//using PdfSharpCore.Pdf;
//using PdfSharpCore.Drawing;
//using System.IO;
//using System;
//using System.Collections.Generic;
//using domain.Model.Cart;
//using core.Interface;

//public class InvoiceGenerator : IInvoiceGenerator
//{
//    public async Task<MemoryStream> GenerateInvoice(int salesMasterId, float totalAmount, List<CartDetail> cartDetails)
//    {
//        var memoryStream = new MemoryStream();

//        // Simulating an async operation for PDF generation
//        await Task.Run(() =>
//        {
//            using (PdfDocument document = new PdfDocument())
//            {

//                PdfPage page = document.AddPage();
//                XGraphics gfx = XGraphics.FromPdfPage(page);


//                XFont boldFont = new XFont("Verdana", 18, XFontStyle.Bold);
//                XFont regularFont = new XFont("Verdana", 12, XFontStyle.Regular);
//                XFont smallFont = new XFont("Verdana", 10, XFontStyle.Regular);

//                gfx.DrawString($"Invoice #{salesMasterId}", boldFont, XBrushes.Black, new XPoint(page.Width / 2, 50), XStringFormats.Center);
//                gfx.DrawString($"Date: {DateTime.UtcNow:dd MMM yyyy}", regularFont, XBrushes.Black, new XPoint(page.Width / 2, 80), XStringFormats.Center);
//                gfx.DrawString($"Total Amount: ₹{totalAmount}", regularFont, XBrushes.Black, new XPoint(page.Width / 2, 110), XStringFormats.Center);

                
//                gfx.DrawString("Product Details", boldFont, XBrushes.Black, new XPoint(50, 150));

                
//                int yPosition = 180;
//                foreach (var detail in cartDetails)
//                {
//                    string productLine = $"{detail.Product.ProductName} - ₹{detail.Product.SellingPrice} x {detail.Quantity} = ₹{detail.Quantity * detail.Product.SellingPrice}";
//                    gfx.DrawString(productLine, regularFont, XBrushes.Black, new XPoint(50, yPosition));
//                    yPosition += 25; 
//                }

//                // Add footer message
//                gfx.DrawString("\nThank you for your purchase!", smallFont, XBrushes.Black, new XPoint(page.Width / 2, yPosition + 20), XStringFormats.Center);

//                // Save to memory stream
//                document.Save(memoryStream);
//                memoryStream.Seek(0, SeekOrigin.Begin);  // Reset the stream to the beginning
//            }
//        });

//        return memoryStream; // Return the MemoryStream wrapped in Task
//    }


//}
