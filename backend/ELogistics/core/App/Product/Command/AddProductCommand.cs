using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using core.API_Response;
using core.Interface;
using domain.ModelDtos;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace core.App.Product.Command
{
    public class AddProductCommand : IRequest<AppResponse<object>>
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public ProductDto Product { get; set; }

    }

    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly string _connectionString;
        private readonly string _containerName = "elogistics";
        private readonly string _folderName = "product-images";
        public AddProductCommandHandler(IAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("AzureBlobStorage");
        }
        public async Task<AppResponse<object>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var requestProduct = request.Product;

            // STORING IMAGES
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient($"{_folderName}/{Guid.NewGuid()}");

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(request.FileName) // Get MIME type dynamically
            };

            await blobClient.UploadAsync(request.FileStream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            var imageUrl = blobClient.Uri.ToString();

            var product = requestProduct.Adapt<domain.Model.Products.Product>();

            product.ProductImageUrl = imageUrl;
            product.ProductCode  = $"PRD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            await _context.Set<domain.Model.Products.Product>().AddAsync(product);

            await _context.SaveChangesAsync();

            return AppResponse.Success<object>(data: product.Adapt<ProductDto>(), message: "Product Added Successfully", statusCode: HttpStatusCodes.Created);
        }

        private string GetContentType(string fileName)
        {
            return fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" :
                   fileName.EndsWith(".png") ? "image/png" :
                   "application/octet-stream"; // Default fallback
        }
    }

}
