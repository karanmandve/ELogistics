using core.API_Response;
using core.Interface;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace core.App.Appointment.Command
{
    public class CanceledAppointmentCommand : IRequest<AppResponse<object>>
    {
        public int AppointmentId { get; set; }
    }

    public class CanceledAppointmentCommandHandler : IRequestHandler<CanceledAppointmentCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IEmailService _emailService;

        public CanceledAppointmentCommandHandler(IAppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(CanceledAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentData = await _appDbContext.Set<domain.Model.Appointment.Appointment>().FindAsync(request.AppointmentId);
            if (appointmentData == null)
            {
                return AppResponse.Fail<object>(message: "Appointment not found", statusCode: HttpStatusCodes.NotFound);
            }

            appointmentData.AppointmentStatus = domain.Model.Appointment.AppointmentStatusEnum.Cancelled.ToString();

            var patient = await _appDbContext.Set<domain.Model.User.User>().FindAsync(appointmentData.PatientId);
            var provider = await _appDbContext.Set<domain.Model.User.User>().FindAsync(appointmentData.ProviderId);

            if (patient == null || provider == null)
            {
                return AppResponse.Fail<object>(message: "Patient or Provider not found", statusCode: HttpStatusCodes.NotFound);
            }

            // Email body for the patient
            var patientEmailBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>
                <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                    <tr>
                        <td align='center' style='background-color: #e63946; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                            <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 18px; color: #e63946;'>Dear {patient.FirstName},</p>
                            <p style='font-size: 16px; color: #333;'>We regret to inform you that your appointment has been canceled. Please find the details below:</p>
                            <p style='font-size: 16px; color: #333;'><strong>Provider:</strong> {provider.FirstName} {provider.LastName}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Date:</strong> {appointmentData.AppointmentDate:dd-MM-yyyy}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Time:</strong> {appointmentData.AppointmentTime}</p>
                            <p style='font-size: 16px; color: #333;'>If you have any questions, please contact us.</p>
                            <p style='font-size: 16px; color: #e63946;'>Best regards,<br>The EHRApplication Team</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #e63946; padding: 20px; color: white; font-size: 14px;'>
                            <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br> Your partner in healthcare.</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            // Email body for the provider
            var providerEmailBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>
                <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                    <tr>
                        <td align='center' style='background-color: #e63946; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                            <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 18px; color: #e63946;'>Dear {provider.FirstName},</p>
                            <p style='font-size: 16px; color: #333;'>The appointment with your patient, {patient.FirstName} {patient.LastName}, has been canceled. Please find the details below:</p>
                            <p style='font-size: 16px; color: #333;'><strong>Patient:</strong> {patient.FirstName} {patient.LastName}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Date:</strong> {appointmentData.AppointmentDate:dd-MM-yyyy}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Time:</strong> {appointmentData.AppointmentTime}</p>
                            <p style='font-size: 16px; color: #333;'>If you have any questions, please contact us.</p>
                            <p style='font-size: 16px; color: #e63946;'>Best regards,<br>The EHRApplication Team</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #e63946; padding: 20px; color: white; font-size: 14px;'>
                            <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br> Your partner in healthcare.</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            // Send emails
            await _emailService.SendEmailAsync(patient.Email, "Appointment Canceled", patientEmailBody);
            await _emailService.SendEmailAsync(provider.Email, "Appointment Canceled", providerEmailBody);

            await _appDbContext.SaveChangesAsync();

            return AppResponse.Success<object>(message: "Successfully Canceled Appointment", statusCode: HttpStatusCodes.OK);
        }
    }
}
