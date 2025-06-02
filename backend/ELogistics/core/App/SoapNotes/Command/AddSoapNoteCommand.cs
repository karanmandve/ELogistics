using core.API_Response;
using core.Interface;
using Mapster;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace core.App.SoapNotes.Command
{
    public class AddSoapNoteCommand : IRequest<AppResponse<object>>
    {
        public domain.ModelDto.SoapNote.SoapNoteDto SoapNote { get; set; }
    }

    public class AddSoapNoteCommandHandler : IRequestHandler<AddSoapNoteCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IEmailService _emailService;

        public AddSoapNoteCommandHandler(IAppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(AddSoapNoteCommand request, CancellationToken cancellationToken)
        {
            var soapNoteData = request.SoapNote.Adapt<domain.Model.SoapNotes.SoapNote>();

            soapNoteData.DateCreated = DateTime.Now;

            var appointment = await _appDbContext.Set<domain.Model.Appointment.Appointment>().FindAsync(soapNoteData.AppointmentId);

            if (appointment == null)
            {
                return AppResponse.Fail<object>(null, message: "Appointment Not Found", statusCode: HttpStatusCodes.NotFound);
            }

            appointment.AppointmentStatus = domain.Model.Appointment.AppointmentStatusEnum.Completed.ToString();

            var patient = await _appDbContext.Set<domain.Model.User.User>().FindAsync(appointment.PatientId);
            var provider = await _appDbContext.Set<domain.Model.User.User>().FindAsync(appointment.ProviderId);

            if (patient == null || provider == null)
            {
                return AppResponse.Fail<object>(null, message: "Patient or Provider Not Found", statusCode: HttpStatusCodes.NotFound);
            }

            await _appDbContext.Set<domain.Model.SoapNotes.SoapNote>().AddAsync(soapNoteData);

            // Email content for the patient
            var patientEmailBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>
                <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                    <tr>
                        <td align='center' style='background-color: #28a745; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                            <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 18px; color: #28a745;'>Dear {patient.FirstName},</p>
                            <p style='font-size: 16px; color: #333;'>Your appointment has been successfully completed. Please find the summary in the SOAP notes:</p>
                            <p style='font-size: 16px; color: #333;'><strong>Provider:</strong> {provider.FirstName} {provider.LastName}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Date:</strong> {appointment.AppointmentDate:dd-MM-yyyy}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Time:</strong> {appointment.AppointmentTime}</p>
                            <p style='font-size: 16px; color: #333;'>Thank you for using our services.</p>
                            <p style='font-size: 16px; color: #28a745;'>Best regards,<br>The EHRApplication Team</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #28a745; padding: 20px; color: white; font-size: 14px;'>
                            <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br> Your partner in healthcare.</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            // Email content for the provider
            var providerEmailBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 0;'>
                <table width='100%' cellpadding='0' cellspacing='0' style='padding: 20px;'>
                    <tr>
                        <td align='center' style='background-color: #28a745; padding: 20px; color: white; font-size: 24px; font-weight: bold;'>
                            <h1 style='margin: 0; color: white;'>EHRApplication</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: white; padding: 40px; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 18px; color: #28a745;'>Dear {provider.FirstName},</p>
                            <p style='font-size: 16px; color: #333;'>The appointment with your patient, {patient.FirstName} {patient.LastName}, has been successfully completed. Please find the SOAP notes added for reference:</p>
                            <p style='font-size: 16px; color: #333;'><strong>Patient:</strong> {patient.FirstName} {patient.LastName}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Date:</strong> {appointment.AppointmentDate:dd-MM-yyyy}</p>
                            <p style='font-size: 16px; color: #333;'><strong>Time:</strong> {appointment.AppointmentTime}</p>
                            <p style='font-size: 16px; color: #333;'>Thank you for your efforts in ensuring patient care.</p>
                            <p style='font-size: 16px; color: #28a745;'>Best regards,<br>The EHRApplication Team</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #28a745; padding: 20px; color: white; font-size: 14px;'>
                            <p>© {DateTime.Now.Year} EHRApplication. All rights reserved. <br> Your partner in healthcare.</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            // Send emails
            await _emailService.SendEmailAsync(patient.Email, "Appointment Completed", patientEmailBody);
            await _emailService.SendEmailAsync(provider.Email, "Appointment Completed", providerEmailBody);

            await _appDbContext.SaveChangesAsync();

            return AppResponse.Success<object>(null, message: "Successfully Added SOAP Note and Notified Parties", statusCode: HttpStatusCodes.OK);
        }
    }
}
