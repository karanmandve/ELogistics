using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using core.API_Response;
using core.Interface;
using domain.ModelDtos;
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
        public RegisterDto RegisterUserData { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(IAppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var registerUser = request.RegisterUserData;

            var userAlreadyExist = await _context.Set<domain.Model.Users.User>().FirstOrDefaultAsync(us => us.Email == registerUser.Email);

            if (userAlreadyExist != null)
            {
                return AppResponse.Fail<object>(message: "User Already Exist", statusCode: HttpStatusCodes.Conflict);
            }

            var user = registerUser.Adapt<domain.Model.Users.User>();
            user.Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);

            await _context.Set<domain.Model.Users.User>().AddAsync(user);
            await _context.SaveChangesAsync(cancellationToken);

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var emailBody = $@"
                            <html>
                            <body style=""font-family: Arial, sans-serif; background-color: #f8f9fa; margin: 0; padding: 0;"">
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding: 20px;"">
                                <tr>
                                    <td align=""center"" style=""background-color: #004085; padding: 30px;"">
                                    <h1 style=""margin: 0; color: #ffffff; font-size: 28px;"">ELogistics</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);"">
                                    <p style=""font-size: 18px; color: #0056b3; margin-bottom: 12px;"">
                                        Dear {textInfo.ToTitleCase(registerUser.FirstName)} {textInfo.ToTitleCase(registerUser.LastName)},
                                    </p>
                                    <p style=""font-size: 16px; color: #333333; line-height: 1.5; margin-bottom: 20px;"">
                                        Welcome to <strong>ELogistics</strong>! Your account has been successfully created.
                                    </p>
                                    <p style=""font-size: 16px; color: #333333; line-height: 1.5; margin-bottom: 20px;"">
                                        We’re thrilled to have you on board and look forward to streamlining your shipping and logistics operations.
                                    </p>
                                    <p style=""font-size: 16px; color: #333333; line-height: 1.5; margin-bottom: 30px;"">
                                        If you have any questions, just reply to this email or reach out to our support team.
                                    </p>
                                    <p style=""font-size: 16px; color: #0056b3; margin: 0;"">
                                        Best regards,<br/>
                                        The ELogistics Team
                                    </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"" style=""background-color: #004085; padding: 20px;"">
                                    <p style=""margin: 0; font-size: 14px; color: #dddddd;"">
                                        © {DateTime.Now.Year} ELogistics. All rights reserved.
                                    </p>
                                    </td>
                                </tr>
                                </table>
                            </body>
                            </html>";

            await _emailService.SendEmailAsync(
                registerUser.Email,
                "Welcome to ELogistics",
                emailBody
            );

            return AppResponse.Success<object>(message: "User Created Successfully", statusCode: HttpStatusCodes.Created);
        }
    }
}
