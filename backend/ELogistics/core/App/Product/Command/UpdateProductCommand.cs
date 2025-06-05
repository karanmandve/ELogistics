using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using core.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using domain.ModelDtos;
using core.API_Response;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

namespace core.App.Product.Command
{
    public class UpdateProductCommand : IRequest<AppResponse<object>>
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public ProductDto Product { get; set; }
    }
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly string _connectionString;
        private readonly string _containerName = "elogistics";
        private readonly string _folderName = "product-images";

        public UpdateProductCommandHandler(IAppDbContext appDbContext, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _context = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productId = request.Product.Id;

            if (productId == null)
                return AppResponse.Fail<object>(message: "Product Id is required", statusCode: HttpStatusCodes.BadRequest);

            var product = await _context.Set<domain.Model.Products.Product>().FindAsync(productId);
            if (product == null)
            {
                return AppResponse.Fail<object>(message: "Product Not Found", statusCode: HttpStatusCodes.NotFound);
            }

            string imageUrl = null;
            if (request.FileStream != null && !string.IsNullOrEmpty(request.FileName))
            {
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

                imageUrl = blobClient.Uri.ToString();
            }
            
            product = request.Product.Adapt(product);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                product.ProductImageUrl = imageUrl;
            }
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return AppResponse.Success<object>(data: null, message: "Product Updated Successfully", statusCode: HttpStatusCodes.OK);
        }

        private string GetContentType(string fileName)
        {
            return fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" :
                   fileName.EndsWith(".png") ? "image/png" :
                   "application/octet-stream"; // Default fallback
        }
    }
}
