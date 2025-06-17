namespace domain.ModelDtos
{ 
    public class AllProductResponseDto
    {
        public Guid Id { get; set; }
        public Guid DistributorId { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public decimal ProductMRP { get; set; }
        public decimal ProductRate { get; set; }
        public int AvailableStocks { get; set; }
    }

}