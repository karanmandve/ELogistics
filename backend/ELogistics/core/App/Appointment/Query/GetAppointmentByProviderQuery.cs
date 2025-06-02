using core.API_Response;
using core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace core.App.Appointment.Query
{
    public class GetAppointmentByProviderQuery : IRequest<AppResponse<object>>
    {
        public int ProviderId { get; set; }
    }

    public class GetAppointmentByProviderQueryHandler : IRequestHandler<GetAppointmentByProviderQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;
        public GetAppointmentByProviderQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<AppResponse<object>> Handle(GetAppointmentByProviderQuery request, CancellationToken cancellationToken)
        {
            var appointmentData = await _appDbContext.Set<domain.Model.Appointment.Appointment>().Where(x => x.ProviderId == request.ProviderId).ToListAsync();
            var appointmentDataResponse = new List<object>();
            foreach (var appointment in appointmentData)
            {
                var patient = await _appDbContext.Set<domain.Model.User.User>().FirstOrDefaultAsync(x => x.Id == appointment.PatientId);

                var age = CalculateAge(patient.DateOfBirth);


                var data = new
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    AppointmentStatus = appointment.AppointmentStatus,
                    ProfileImageUrl = patient.ProfileImageUrl,
                    Age = age,
                    PatientName = patient.FirstName + " " + patient.LastName,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    ChiefComplaint = appointment.ChiefComplaint,
                    Fee = appointment.Fee,
                };
                appointmentDataResponse.Add(data);
            }

            return AppResponse.Success<object>(appointmentDataResponse, message: "Successfully Get Appointment", statusCode: HttpStatusCodes.OK);
        }

        int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
