using Microsoft.AspNetCore.Http;

namespace domain.ModelDtos
{
    public class ProductDto
    {
        public Guid UserId { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string ProductName { get; set; }

        public string? ProductCategory { get; set; }

        public decimal ProductMRP { get; set; }

        public decimal ProductRate { get; set; }

        public int AvailableStocks { get; set; }
    }
} 