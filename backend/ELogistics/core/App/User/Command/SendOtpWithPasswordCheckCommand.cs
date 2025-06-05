using core.API_Response;
using core.Interface;
using domain.Model.Otp;
using domain.ModelDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace core.App.User.Command
{
    public class SendOtpWithPasswordCheckCommand : IRequest<AppResponse<object>>
    {
        public SendOtpWithPasswordCheckDto SendOtpWithPasswordCheck { get; set; }
    }

    public class SendOtpWithPasswordCheckCommandHandler : IRequestHandler<SendOtpWithPasswordCheckCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IEmailService _emailService;

        public SendOtpWithPasswordCheckCommandHandler(IAppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(SendOtpWithPasswordCheckCommand request, CancellationToken cancellationToken)
        {
            var email = request.SendOtpWithPasswordCheck.Email;
            var password = request.SendOtpWithPasswordCheck.Password;

            // Try Distributor first
            var distributor = await _context.Set<domain.Model.Users.Distributor>()
                .FirstOrDefaultAsync(x => x.Email == email);

            // Try Customer if distributor not found
            var customer = distributor == null ? 
                await _context.Set<domain.Model.Users.Customer>()
                    .FirstOrDefaultAsync(x => x.Email == email) : null;

            // Check if either user exists and password matches
            if ((distributor == null && customer == null) || 
                (distributor != null && !BCrypt.Net.BCrypt.Verify(password, distributor.Password)) ||
                (customer != null && !BCrypt.Net.BCrypt.Verify(password, customer.Password)))
            {
                return AppResponse.Fail<object>(message: "Invalid Email or Password", statusCode: HttpStatusCodes.NotFound);
            }

            dynamic user = distributor != null ? distributor : customer;
            
            var otp = new Random().Next(100000, 999999).ToString();

            await _context.Set<Otp>().AddAsync(new domain.Model.Otp.Otp 
            { 
                Email = user.Email, 
                Code = otp, 
                Expiration = DateTime.Now.AddMinutes(5) 
            });
            await _context.SaveChangesAsync();

            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            var emailBody = $@"
                            <html>
                              <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; margin: 0; padding: 0;'>
                                <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                                  <tr>
                                    <td align='center' style='background-color: #004085; padding: 30px; color: #ffffff; text-align: center;'>
                                      <h1 style='margin: 0; font-size: 30px;'>ELogistics</h1>
                                    </td>
                                  </tr>
                                  <tr>
                                    <td style='background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);'>
                                      <p style='font-size: 18px; color: #0056b3; margin-bottom: 20px;'>
                                        Dear {textInfo.ToTitleCase(user.FirstName)} {textInfo.ToTitleCase(user.LastName)},
                                      </p>
                                      <p style='font-size: 16px; color: #333333; margin-bottom: 20px;'>
                                        Thank you for using our service. Your one-time passcode (OTP) is:
                                      </p>
                                      <p style='font-size: 24px; color: #333333; font-weight: bold; margin: 20px 0; padding: 12px 20px; border-radius: 6px; background-color: #e1f5fe; display: inline-block;'>
                                        {otp}
                                      </p>
                                      <p style='font-size: 16px; color: #333333; margin-top: 20px;'>
                                        Please use this OTP within the next 5 minutes. If you did not request this, simply ignore this email.
                                      </p>
                                      <p style='font-size: 16px; color: #0056b3; margin-top: 30px;'>
                                        Best regards,<br/>
                                        The ELogistics Team
                                      </p>
                                    </td>
                                  </tr>
                                  <tr>
                                    <td align='center' style='background-color: #004085; padding: 20px; color: #dddddd; font-size: 14px;'>
                                      <p style='margin: 0;'>
                                        © {DateTime.Now.Year} ELogistics. All rights reserved.
                                      </p>
                                    </td>
                                  </tr>
                                </table>
                              </body>
                            </html>";

            await _emailService.SendEmailAsync(
                user.Email,
                "Your OTP for ELogistics",
                emailBody
            );

            return AppResponse.Success<object>(message: "OTP sent to your email", statusCode: HttpStatusCodes.OK);
        }
    }
}