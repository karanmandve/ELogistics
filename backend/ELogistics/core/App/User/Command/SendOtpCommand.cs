/*
using core.API_Response;
using core.Interface;
using domain.Model.Otp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace core.App.User.Command
{
    public class SendOtpCommand : IRequest<AppResponse<object>>
    {
        public string Username { get; set; }
    }

    //public class UserLoginQuery : IRequest<Object>
    //{
    //    public domain.ModelDto.LoginDto LoginUser { get; set; }
    //    public string Otp { get; set; } // Add OTP property
    //}

    public class SendOtpQueryHandler : IRequestHandler<SendOtpCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService; // Add email service

        public SendOtpQueryHandler(IAppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var username = request.Username;

            var existingUser = await _context.Set<domain.Model.User.User>().FirstOrDefaultAsync(x => x.Username == username);

            if (existingUser == null)
            {
                return AppResponse.Fail<object>(message: "User Not Exist", statusCode: HttpStatusCodes.NotFound);
            }


            var otp = new Random().Next(100000, 999999).ToString();

            await _context.Set<Otp>().AddAsync(new domain.Model.Otp.Otp { Username = existingUser.Username, Code = otp, Expiration = DateTime.Now.AddMinutes(5) });
            await _context.SaveChangesAsync();


            var emailBody = $@"
                                <html>
                                  <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>
                                    <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                                      <tr>
                                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 28px; font-weight: bold; text-align: center;'>
                                          <h1 style='margin: 0; color: white; font-size: 30px;'>EHRApplication</h1>
                                        </td>
                                      </tr>
                                      <tr>
                                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                                          <p style='font-size: 18px; color: #0077b6; margin-bottom: 20px;'>Dear {existingUser.FirstName},</p>
                                          <p style='font-size: 16px; color: #333; margin-bottom: 20px;'>Thank you for using our service. Your OTP is:</p>
                                          <p style='font-size: 24px; color: #333; font-weight: bold; margin: 20px 0; padding: 10px; border-radius: 5px; background-color: #e1f5fe; display: inline-block;'>{otp}</p>
                                          <p style='font-size: 16px; color: #333; margin-top: 20px;'>Please use this OTP within the next 5 minutes. If you did not request this, please ignore this email.</p>
                                          <p style='font-size: 16px; color: #0077b6; margin-top: 20px;'>Best regards,<br>The EHRApplication Team</p>
                                        </td>
                                      </tr>
                                      <tr>
                                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 14px;'>
                                          <p>© {DateTime.Now.Year} EHRApplication. All rights reserved.<br>Your partner in healthcare.</p>
                                        </td>
                                      </tr>
                                    </table>
                                  </body>
                                </html>";


            await _emailService.SendEmailAsync(
                existingUser.Email,
                "Your OTP for EHRApplication",
                emailBody
            );

            return AppResponse.Success<object>(message: "OTP sent to your email", statusCode: HttpStatusCodes.OK);
        }
    }
}
*/
