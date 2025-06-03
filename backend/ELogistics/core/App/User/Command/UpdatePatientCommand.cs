/*
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using core.API_Response;
using core.Interface;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace core.App.Users.Command
{
    public class UpdatePatientCommand : IRequest<AppResponse<object>>
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public domain.ModelDto.UpdatePatientDto User { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdatePatientCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly string _connectionString;
        private readonly string _containerName = "ehrapplication";
        private readonly string _folderName = "profile-images";

        public UpdateUserCommandHandler(IAppDbContext appDbContext, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _context = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var id = request.User.Id;

            var user = await _context.Set<domain.Model.User.User>().FindAsync(id);

            if (user == null)
            {
                return AppResponse.Fail<object>(message: "User Not Found", statusCode: HttpStatusCodes.NotFound);
            }

            var imageUrl = string.Empty;

            if (request.FileName != null)
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

            user.FirstName = request.User.FirstName;
            user.LastName = request.User.LastName;
            user.Mobile = request.User.Mobile;
            user.DateOfBirth = request.User.DateOfBirth;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                user.ProfileImageUrl = imageUrl;
            }
            user.Gender = request.User.Gender;
            user.BloodGroup = request.User.BloodGroup;
            user.Address = request.User.Address;
            user.City = request.User.City;
            user.StateId = request.User.StateId;
            user.CountryId = request.User.CountryId;
            user.Pincode = request.User.Pincode;

            await _context.SaveChangesAsync(cancellationToken);

            var userData = user.Adapt<domain.ModelDto.UserDto>();

            return AppResponse.Success<object>(data: userData, message: "User Updated Successfully", statusCode: HttpStatusCodes.OK);
        }

        private string GetContentType(string fileName)
        {
            return fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" :
                   fileName.EndsWith(".png") ? "image/png" :
                   "application/octet-stream"; // Default fallback
        }
    }
}
*/
