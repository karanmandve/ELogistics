using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using core.API_Response;
using core.Interface;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PasswordGenerator;
using System.Globalization;

namespace core.App.User.Command
{
    public class CreateUserCommand : IRequest<AppResponse<object>>
    {
        public domain.ModelDto.RegisterDto RegisterUser { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly string _connectionString;
        private readonly string _containerName = "ehrapplication";
        private readonly string _folderName = "profile-images";
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(IAppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var registerUser = request.RegisterUser;

            var userAlreadyExist = await _context.Set<domain.Model.User.User>().FirstOrDefaultAsync(us => us.Email == registerUser.Email);

            if (userAlreadyExist != null)
            {
                return AppResponse.Fail<object>(message: "User Already Exist", statusCode: HttpStatusCodes.Conflict);
            }

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


            // Generate Username
            string formattedDOB = registerUser.DateOfBirth.ToString("ddMMyy");

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var username = "";

            if (registerUser.UserTypeId == 1)
            {
                username = $"PR_{textInfo.ToTitleCase(registerUser.LastName)}{registerUser.FirstName.ToUpper()[0]}{formattedDOB}";
            }
            else
            {
                username = $"PT_{textInfo.ToTitleCase(registerUser.LastName)}{registerUser.FirstName.ToUpper()[0]}{formattedDOB}";
            }

            var existingUsername = await _context.Set<domain.Model.User.User>().FirstOrDefaultAsync(x => x.Username == username);


            if (existingUsername != null)
            {
                username = $"{existingUsername}1";
            }

            // Generate Password
            var passwordGenerator = new Password(true, true, true, true, 13);
            string password = passwordGenerator.Next();
            password = password.Replace("\\", "*");
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(password, 13);


            var user = registerUser.Adapt<domain.Model.User.User>();

            user.Username = username;
            user.Password = hashPassword;
            user.ProfileImageUrl = imageUrl;
            user.IsDeleted = false;
            user.DateCreated = DateTime.Now;

            await _context.Set<domain.Model.User.User>().AddAsync(user);
            await _context.SaveChangesAsync(cancellationToken);

            var emailBody = $"<html><body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>" +
                $"<table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>" +
                $"  <tr>" +
                $"    <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>" +
                $"      <h1 style='margin: 0; color: white;'>EHRApplication</h1>" +
                $"    </td>" +
                $"  </tr>" +
                $"  <tr>" +
                $"    <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>" +
                $"      <p style='font-size: 18px; color: #0077b6;'>Dear {registerUser.FirstName},</p>" +
                $"      <p style='font-size: 16px; color: #333;'>Congratulations! Your account has been successfully created on <strong>EHRApplication</strong>. We're committed to empowering your healthcare journey.</p>" +
                $"      <p style='font-size: 16px; color: #333;'><strong>Your account details:</strong></p>" +
                $"      <p style='font-size: 16px; color: #333;'><strong>Username:</strong> {username}</p>" +
                $"      <p style='font-size: 16px; color: #333;'><strong>Password:</strong> {password}</p>" +
                $"      <p style='font-size: 16px; color: #333;'>Please keep your credentials secure. If you did not create this account, contact our support team immediately.</p>" +
                $"      <p style='font-size: 16px; color: #333;'>Thank you for trusting <strong>EHRApplication</strong>. Together, we aim to enhance healthcare experiences.</p>" +
                $"      <p style='font-size: 16px; color: #0077b6;'>Best regards,<br>The EHRApplication Team</p>" +
                $"    </td>" +
                $"  </tr>" +
                $"  <tr>" +
                $"    <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 14px;'>" +
                $"      <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br> Your partner in healthcare.</p>" +
                $"    </td>" +
                $"  </tr>" +
                $"</table>" +
                $"</body></html>";

            await _emailService.SendEmailAsync(
                registerUser.Email,
                "Welcome to EHRApplication",
                emailBody

            );

            return AppResponse.Success<object>(message: "User Created Successfully", statusCode: HttpStatusCodes.Created);

        }

        private string GetContentType(string fileName)
        {
            return fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" :
                   fileName.EndsWith(".png") ? "image/png" :
                   "application/octet-stream";
        }
    }
}
