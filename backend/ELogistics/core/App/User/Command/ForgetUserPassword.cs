/*
using core.API_Response;
using core.Interface;
using domain.Model.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.User.Command
{
    public class ForgetUserPassword : IRequest<AppResponse<object>>
    {
        public string Username { get; set; }
        public string Otp { get; set; }
    }

    public class ForgetUserPasswordHandler : IRequestHandler<ForgetUserPassword, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IEmailService _emailService;

        public ForgetUserPasswordHandler(IAppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(ForgetUserPassword request, CancellationToken cancellationToken)
        {
            var username = request.Username;
            var requestOtp = request.Otp;

            var existingCustomer = await _context.Set<domain.Model.Users.Customer>().FirstOrDefaultAsync(x => x.Username == username);

            var otp = await _context.Set<domain.Model.Otp.Otp>().FirstOrDefaultAsync(x => x.Username == username && x.Code == requestOtp && x.Expiration > DateTime.Now);

            if (existingCustomer == null || otp == null)
            {
                return AppResponse.Fail<object>(message: "Invalid OTP", statusCode: HttpStatusCodes.Unauthorized);
            }

            // Generate new password
            var passwordGenerator = new Password(true, true, true, true, 13);
            string password = passwordGenerator.Next();
            password = password.Replace("\\", "*");

            existingCustomer.Password = BCrypt.Net.BCrypt.HashPassword(password);

            await _context.SaveChangesAsync();

            var emailBody = $@"
                                                <html>
                                                  <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>
                                                    <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                                                      <tr>
                                                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                                                          <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                                                        </td>
                                                      </tr>
                                                      <tr>
                                                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                                                          <p style='font-size: 18px; color: #0077b6;'>Dear {existingCustomer.FirstName},</p>
                                                          <p style='font-size: 16px; color: #333;'>We have received your request to reset your password. Your new password has been successfully generated.</p>
                                                          <p style='font-size: 16px; color: #333;'><strong>Your new password:</strong> <span style='color: #0077b6;'>{password}</span></p>
                                                          <p style='font-size: 16px; color: #333;'>Please log in to your account and change this password as soon as possible for security purposes.</p>
                                                          <p style='font-size: 16px; color: #333;'>If you did not request a password reset, please contact our support team immediately.</p>
                                                          <p style='font-size: 16px; color: #0077b6;'>Best regards,<br>The EHRApplication Team</p>
                                                        </td>
                                                      </tr>
                                                      <tr>
                                                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 14px;'>
                                                          <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br>Your partner in healthcare.</p>
                                                        </td>
                                                      </tr>
                                                    </table>
                                                  </body>
                                                </html>";


            await _emailService.SendEmailAsync(existingCustomer.Email,
                "Password Reset Successful",
                emailBody
            );

            var allOtps = await _context.Set<domain.Model.Otp.Otp>().Where(x => x.Username == username).ToListAsync();
            _context.Set<domain.Model.Otp.Otp>().RemoveRange(allOtps);

            await _context.SaveChangesAsync();

            return AppResponse.Success<object>(message: "Password reset successful", statusCode: HttpStatusCodes.OK);
        }
    }
}
*/
