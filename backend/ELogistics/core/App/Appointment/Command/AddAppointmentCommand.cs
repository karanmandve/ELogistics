using core.API_Response;
using core.Interface;
using Mapster;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace core.App.Appointment.Command
{
    public class AddAppointmentCommand : IRequest<AppResponse<object>>
    {
        public domain.ModelDto.Appointment.AppointmentDto Appointment { get; set; }
    }

    public class AddAppointmentCommandHandler : IRequestHandler<AddAppointmentCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IRazorpayService _razorpayService;
        private readonly IEmailService _emailService;

        public AddAppointmentCommandHandler(
            IAppDbContext appDbContext,
            IRazorpayService razorpayService,
            IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _razorpayService = razorpayService;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
        {
            if (request.Appointment.UserTypeId == 2)
            {
                await _razorpayService.VerifyPaymentAsync(request.Appointment.PaymentId, request.Appointment.OrderId);
            }

            var appointmentData = request.Appointment.Adapt<domain.Model.Appointment.Appointment>();

            if (appointmentData == null)
            {
                return AppResponse.Fail<object>(message: "Appointment Data is Null", statusCode: HttpStatusCodes.NotFound);
            }

            appointmentData.AppointmentStatus = domain.Model.Appointment.AppointmentStatusEnum.Scheduled.ToString();

            await _appDbContext.Set<domain.Model.Appointment.Appointment>().AddAsync(appointmentData);
            await _appDbContext.SaveChangesAsync();

            // Fetch patient and provider details
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
                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                            <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 18px; color: #0077b6;'>Dear {patient.FirstName},</p>
                            <p style='font-size: 16px; color: #333;'>Your appointment has been successfully scheduled. Please find the details below:</p>
                            <p style='font-size: 16px; color: #333;'><strong>Provider:</strong> {provider.FirstName} {provider.LastName}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Date:</strong> {appointmentData.AppointmentDate:dd-MM-yyyy}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Time:</strong> {appointmentData.AppointmentTime}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Chief Complaint:</strong> {appointmentData.ChiefComplaint}</p>
                            <p style='font-size: 16px; color: #333;'>If you have any questions, please contact us.</p>
                            <p style='font-size: 16px; color: #0077b6;'>Best regards,<br>The EHRApplication Team</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 14px;'>
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
                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                            <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 18px; color: #0077b6;'>Dear {provider.FirstName},</p>
                            <p style='font-size: 16px; color: #333;'>You have a new appointment with {patient.FirstName} {patient.LastName}. Please find the details below:</p>
                            <p style='font-size: 16px; color: #333;'><strong>Patient:</strong> {patient.FirstName} {patient.LastName}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Date:</strong> {appointmentData.AppointmentDate:dd-MM-yyyy}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Time:</strong> {appointmentData.AppointmentTime}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Chief Complaint:</strong> {appointmentData.ChiefComplaint}</p>
                            <p style='font-size: 16px; color: #333;'>If you have any questions, please contact us.</p>
                            <p style='font-size: 16px; color: #0077b6;'>Best regards,<br>The EHRApplication Team</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #0077b6; padding: 20px; color: white; font-size: 14px;'>
                            <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br> Your partner in healthcare.</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            // Send emails
            await _emailService.SendEmailAsync(patient.Email, "Appointment Scheduled", patientEmailBody);
            await _emailService.SendEmailAsync(provider.Email, "New Appointment Scheduled", providerEmailBody);

            return AppResponse.Success<object>(message: "Successfully Added Appointment", statusCode: HttpStatusCodes.OK);
        }
    }
}
