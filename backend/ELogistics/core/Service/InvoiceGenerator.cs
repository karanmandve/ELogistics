using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using domain.Model.Sales;
using domain.Model.Users;
using core.Interface;

namespace core.Services
{
    public class InvoiceGenerator : IInvoiceGenerator
    {
        public Task<MemoryStream> GenerateInvoice(Guid invoiceId, List<SalesDetail> salesDetail, Customer customerData, Distributor distributorData)
        {
            // A4 size in points: 595 x 842
            const int pageWidth = 595;
            const int pageHeight = 842;
            const int margin = 24; // reduced margin

            var document = new PdfDocument();
            var page = document.AddPage();
            page.Width = pageWidth;
            page.Height = pageHeight;

            var gfx = XGraphics.FromPdfPage(page);

            // Reduced font sizes for compact layout
            var fontRegular = new XFont("Arial", 8, XFontStyle.Regular);
            var fontSmall = new XFont("Arial", 7, XFontStyle.Regular);
            var fontBold = new XFont("Arial", 8, XFontStyle.Bold);
            var fontTitle = new XFont("Arial", 14, XFontStyle.Bold);
            var fontSubtitle = new XFont("Arial", 9, XFontStyle.Regular);
            var fontTotal = new XFont("Arial", 12, XFontStyle.Bold);

            double y = margin;

            // Header
            gfx.DrawString($"GSTIN : {distributorData.GSTNumber ?? "N/A"}", fontSmall, XBrushes.Black, new XRect(margin, y, 160, 10), XStringFormats.TopLeft);
            gfx.DrawString("Subject to Nagpur Jurisdiction", fontSmall, XBrushes.Black, new XRect(0, y, pageWidth, 10), XStringFormats.TopCenter);
            gfx.DrawString($"Mobile: {distributorData.PhoneNumber}", fontBold, XBrushes.Black, new XRect(-margin, y, pageWidth, 10), XStringFormats.TopRight);
            y += 13;

            // Title (Distributor Name)
            gfx.DrawString(distributorData.FirstName + " " + distributorData.LastName, fontTitle, XBrushes.Black, new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
            y += 18;
            // Distributor Address
            string distributorAddress = $"{distributorData.Line1} {distributorData.Line2 ?? ""} {distributorData.City} {distributorData.Zip}".Trim();
            gfx.DrawString(distributorAddress, fontSubtitle, XBrushes.Black, new XRect(0, y, pageWidth, 12), XStringFormats.TopCenter);
            y += 12;
            gfx.DrawString("TAX INVOICE", fontSubtitle, XBrushes.Black, new XRect(0, y, pageWidth, 12), XStringFormats.TopCenter);
            y += 16;

            // Parties Section
            double leftBoxWidth = pageWidth * 0.6 - margin;
            double rightBoxWidth = pageWidth * 0.35 - margin;
            double sectionTop = y;

            // Left (Customer)
            gfx.DrawString($"To, {customerData.FirstName} {customerData.LastName}", fontBold, XBrushes.Black, new XRect(margin, y, leftBoxWidth, 10), XStringFormats.TopLeft);
            y += 10;
            string customerAddress = $"{customerData.Line1} {customerData.Line2 ?? ""} {customerData.City} {customerData.Zip}".Trim();
            gfx.DrawString(customerAddress, fontRegular, XBrushes.Black, new XRect(margin, y, leftBoxWidth, 10), XStringFormats.TopLeft);
            y += 10;
            gfx.DrawString($"Party’s GSTIN : {customerData.GSTNumber ?? "N/A"}", fontSmall, XBrushes.Black, new XRect(margin, y, leftBoxWidth, 8), XStringFormats.TopLeft);

            // Right (Invoice Info)
            // Make the right box just a bit longer than the previous (but not as wide as before)
            double newRightBoxWidth = pageWidth * 0.52 - margin; // slightly longer than 0.45, less than 0.55
            double newRightBoxX = pageWidth - newRightBoxWidth - margin;
            double newRightBoxY = sectionTop;
            double newRightBoxHeight = 26;
            gfx.DrawRectangle(XPens.Black, newRightBoxX, newRightBoxY, newRightBoxWidth, newRightBoxHeight);
            // Invoice No. (full invoiceId for fit)
            gfx.DrawString($"Invoice No. : {invoiceId}", fontBold, XBrushes.Black, new XRect(newRightBoxX + 3, newRightBoxY + 1, newRightBoxWidth - 6, 10), XStringFormats.TopLeft);
            gfx.DrawString("Credit Memo", fontBold, XBrushes.Black, new XRect(newRightBoxX + 3, newRightBoxY + 1, newRightBoxWidth - 6, 10), XStringFormats.TopRight);
            gfx.DrawString($"Date : {DateTime.Now:dd-MM-yyyy}", fontRegular, XBrushes.Black, new XRect(newRightBoxX + 3, newRightBoxY + 13, newRightBoxWidth - 6, 10), XStringFormats.TopLeft);

            y = Math.Max(y, newRightBoxY + newRightBoxHeight) + 10;

            // Table Headers
            double tableX = margin;
            double tableY = y;
            // Adjusted column widths to fill the available width (pageWidth - 2 * margin = 547)
            double[] colWidths = { 30, 140, 50, 40, 50, 45, 55, 45, 55, 37 };
            double totalTableWidth = colWidths.Sum();
            // If needed, scale columns to fit exactly
            double scale = (pageWidth - 2 * margin) / totalTableWidth;
            for (int i = 0; i < colWidths.Length; i++)
                colWidths[i] = Math.Round(colWidths[i] * scale, 2);
            string[] headers = { "Sl.", "Products", "M.R.P.", "Qty.", "Rate", "CGST (%)", "CGST Amt.", "SGST (%)", "SGST Amt.", "Amount" };

            double colX = tableX;
            XSolidBrush headerBgBrush = new XSolidBrush(XColor.FromArgb(0xCC, 0xFF, 0xFF)); // #CCFFFF
            for (int i = 0; i < headers.Length; i++)
            {
                // Draw background color for header
                gfx.DrawRectangle(headerBgBrush, colX, tableY, colWidths[i], 13);
                // Draw border
                gfx.DrawRectangle(XPens.Black, colX, tableY, colWidths[i], 13);
                // Center align all headers
                gfx.DrawString(headers[i], fontRegular, XBrushes.Black, new XRect(colX, tableY, colWidths[i], 13), XStringFormats.Center);
                colX += colWidths[i];
            }
            y = tableY + 13;

            // Table Rows
            int sl = 1;
            decimal totalMRP = 0, subTotal = 0, totalCGST = 0, totalSGST = 0, totalQty = 0;
            foreach (var item in salesDetail)
            {
                colX = tableX;
                double rowHeight = 13;
                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[0], rowHeight);
                gfx.DrawString(sl.ToString(), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[0], rowHeight), XStringFormats.Center);
                colX += colWidths[0];

                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[1], rowHeight);
                // ProductName content left aligned
                gfx.DrawString(item.ProductName, fontRegular, XBrushes.Black, new XRect(colX + 2, y, colWidths[1] - 4, rowHeight), XStringFormats.CenterLeft);
                colX += colWidths[1];

                // Center align all other columns
                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[2], rowHeight);
                gfx.DrawString(item.ProductMRP.ToString("0.00"), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[2], rowHeight), XStringFormats.Center);
                colX += colWidths[2];

                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[3], rowHeight);
                gfx.DrawString(item.SaleQuantity.ToString(), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[3], rowHeight), XStringFormats.Center);
                colX += colWidths[3];

                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[4], rowHeight);
                gfx.DrawString(item.ProductRate.ToString("0.00"), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[4], rowHeight), XStringFormats.Center);
                colX += colWidths[4];

                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[5], rowHeight);
                gfx.DrawString($"{item.CGST}%", fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[5], rowHeight), XStringFormats.Center);
                colX += colWidths[5];

                decimal cgstAmt = item.TotalAmount * item.CGST / 100;
                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[6], rowHeight);
                gfx.DrawString(cgstAmt.ToString("0.00"), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[6], rowHeight), XStringFormats.Center);
                colX += colWidths[6];

                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[7], rowHeight);
                gfx.DrawString($"{item.SGST}%", fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[7], rowHeight), XStringFormats.Center);
                colX += colWidths[7];

                decimal sgstAmt = item.TotalAmount * item.SGST / 100;
                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[8], rowHeight);
                gfx.DrawString(sgstAmt.ToString("0.00"), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[8], rowHeight), XStringFormats.Center);
                colX += colWidths[8];

                gfx.DrawRectangle(XPens.Black, colX, y, colWidths[9], rowHeight);
                gfx.DrawString(item.TotalAmount.ToString("0.00"), fontRegular, XBrushes.Black, new XRect(colX, y, colWidths[9], rowHeight), XStringFormats.Center);

                y += rowHeight;
                sl++;

                totalMRP += item.ProductMRP * item.SaleQuantity;
                subTotal += item.TotalAmount;
                totalCGST += cgstAmt;
                totalSGST += sgstAmt;
                totalQty += item.SaleQuantity;
            }

            // Fill empty rows for spacing
            int minRows = 10; // more rows for compact look
            int emptyRows = Math.Max(0, minRows - salesDetail.Count);
            for (int i = 0; i < emptyRows; i++)
            {
                colX = tableX;
                double rowHeight = 13;
                // Draw left border
                gfx.DrawLine(XPens.Black, colX, y, colX, y + rowHeight);
                // Draw right border
                gfx.DrawLine(XPens.Black, colX + colWidths.Sum(), y, colX + colWidths.Sum(), y + rowHeight);
                // Draw top border only for the first empty row
                if (i == 0)
                    gfx.DrawLine(XPens.Black, colX, y, colX + colWidths.Sum(), y);
                // Draw bottom border only for the last empty row
                if (i == emptyRows - 1 && emptyRows > 0)
                    gfx.DrawLine(XPens.Black, colX, y + rowHeight, colX + colWidths.Sum(), y + rowHeight);
                y += rowHeight;
            }

            // Add extra space before the inline summary
            y += 6;

            // Inline summary
            gfx.DrawString($"MRP Value: {totalMRP:0.00}", fontBold, XBrushes.Black, new XRect(margin, y, 80, 10), XStringFormats.TopLeft);
            gfx.DrawString($"Total Qty: {totalQty}", fontBold, XBrushes.Black, new XRect(pageWidth / 2 - 30, y, 80, 10), XStringFormats.TopCenter);
            gfx.DrawString($"Sub Total: {subTotal:0.00}", fontBold, XBrushes.Black, new XRect(-margin, y, pageWidth, 10), XStringFormats.TopRight);
            y += 10;

            // Add extra space after the inline summary before the line
            y += 4;
            // Draw a horizontal line after the summary
            gfx.DrawLine(XPens.Black, margin, y, pageWidth - margin, y);
            y += 4;

            // Sale summary
            gfx.DrawString($"Sale @ 18.00% => {subTotal:0.00}   CGST : {totalCGST:0.00}   SGST : {totalSGST:0.00}", fontBold, XBrushes.Black, new XRect(margin, y, pageWidth - 2 * margin, 10), XStringFormats.TopLeft);
            y += 12; // Add spacing to prevent overlap
            // GST and Total (right-aligned, on same line)
            decimal gstAmt = totalCGST + totalSGST;

            // Footer notes (vertical, one per line, immediately below sale summary)
            // Draw notes and GST/Total Amt. on the same lines

            double notesX = margin;
            double notesWidth = pageWidth - 2 * margin - 220; // leave space for GST/Total Amt. box

            double boxX = pageWidth - margin - 200;
            double boxWidth = 200;
            double boxY = y;

            // Note 1
            gfx.DrawString("1. Goods once sold will not be taken back.", fontSmall, XBrushes.Black, new XRect(notesX, y, notesWidth, 8), XStringFormats.TopLeft);
            y += 9;
            // Note 2
            gfx.DrawString("2. Cheque Bouncing Charges => 400/-.", fontSmall, XBrushes.Black, new XRect(notesX, y, notesWidth, 8), XStringFormats.TopLeft);
            y += 9;
            // Note 3
            gfx.DrawString("3. Payment made after 15 days of this bill entitled interest @ 24% per annum.", fontSmall, XBrushes.Black, new XRect(notesX, y, notesWidth, 8), XStringFormats.TopLeft);

            // Draw GST Amt. and Total Amt. box aligned to the top of the notes
            // GST Amt.
            gfx.DrawString($"GST Amt.: {gstAmt:0.00}", fontBold, XBrushes.Black, new XRect(boxX, boxY, boxWidth, 10), XStringFormats.TopRight);
            // Total Amt. below GST Amt.
            gfx.DrawString($"Total Amt. ₹ {(subTotal + gstAmt):0.00}", fontTotal, XBrushes.Black, new XRect(boxX, boxY + 14, boxWidth, 14), XStringFormats.TopRight);

            y += 9; // after note 3
            // Draw a horizontal line after the last note and totals
            gfx.DrawLine(XPens.Black, margin, y + 10, pageWidth - margin, y + 10);
            y += 14;
            // Amount in words (bold, with Rs.)
            int totalAmountInt = (int)(subTotal + gstAmt);
            string amountInWords = NumberToWords(totalAmountInt) + " Rupees Only";
            gfx.DrawString($"Rs. {amountInWords}", fontBold, XBrushes.Black, new XRect(margin, y, pageWidth - 2 * margin, 12), XStringFormats.TopLeft);
            y += 14;

            // Output to MemoryStream
            var stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            return Task.FromResult(stream);
        }

        // Helper: Convert number to words (simple version)
        private string NumberToWords(int number)
        {
            // You can use a more robust implementation as needed
            if (number == 0)
                return "Zero";
            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }
            if (number > 0)
            {
                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words.Trim();
        }
    }
}
