using core.API_Response;
using core.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.Appointment.Command
{
    public class UpdateAppointmentCommand : IRequest<AppResponse<object>>
    {
        public domain.ModelDto.Appointment.UpdateAppointmentDto Appointment { get; set; }
    }

    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IEmailService _emailService;

        public UpdateAppointmentCommandHandler(IAppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentData = await _appDbContext.Set<domain.Model.Appointment.Appointment>().FindAsync(request.Appointment.Id);

            if (appointmentData == null && appointmentData.AppointmentStatus == "Canceled")
            {
                return AppResponse.Fail<object>(message: "Appointment not found", statusCode: HttpStatusCodes.NotFound);
            }

            appointmentData.AppointmentDate = request.Appointment.AppointmentDate;
            appointmentData.AppointmentTime = request.Appointment.AppointmentTime;
            appointmentData.ChiefComplaint = request.Appointment.ChiefComplaint;

            // send mail to patient and provider both that appointment is changed we can get patientId and providerId from appointmentData and then fetch user table for that and make html script like we use in createUserCommandHandler

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
                            <p style='font-size: 16px; color: #333;'>Your appointment has been updated successfully. Please find the updated details below:</p>
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
                            <p style='font-size: 16px; color: #333;'>The appointment with your patient, {patient.FirstName} {patient.LastName}, has been updated successfully. Please find the updated details below:</p>
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
            await _emailService.SendEmailAsync(patient.Email, "Appointment Update", patientEmailBody);
            await _emailService.SendEmailAsync(provider.Email, "Appointment Update", providerEmailBody);

            await _appDbContext.SaveChangesAsync();

            return AppResponse.Success<object>(message: "Successfully Update Appointment", statusCode: HttpStatusCodes.OK);
        }
    }


}
