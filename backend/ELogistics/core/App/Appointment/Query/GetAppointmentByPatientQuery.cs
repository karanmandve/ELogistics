using core.API_Response;
using core.Interface;
using domain.Model.Appointment;
using domain.ModelDto.Appointment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.Appointment.Query
{
    public class GetAppointmentByPatientQuery : IRequest<AppResponse<object>>
    {
        public int PatientId { get; set; }
    }

    public class GetAppointmentByPatientQueryHandler : IRequestHandler<GetAppointmentByPatientQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAppointmentByPatientQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(GetAppointmentByPatientQuery request, CancellationToken cancellationToken)
        {
            var appointmentData = await _appDbContext.Set<domain.Model.Appointment.Appointment>().Where(x => x.PatientId == request.PatientId).ToListAsync();

            var appointmentDataResponse = new List<object>();

            foreach (var appointment in appointmentData)
            {
                var provider = await _appDbContext.Set<domain.Model.User.User>().FirstOrDefaultAsync(x => x.Id == appointment.ProviderId);
                var specialisation = await _appDbContext.Set<domain.Model.User.Specialisation>().FirstOrDefaultAsync(x => x.Id == provider.SpecialisationId);
                var data = new
                {
                    Id = appointment.Id,
                    AppointmentStatus = appointment.AppointmentStatus,
                    ProviderId = appointment.ProviderId,
                    ProviderName = provider.FirstName + " " + provider.LastName,
                    SpecialisationName = specialisation.SpecialisationName,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    ChiefComplaint = appointment.ChiefComplaint,
                    Fee = appointment.Fee,
                };

                appointmentDataResponse.Add(data);
            }

            return AppResponse.Success<object>(appointmentDataResponse, message: "Successfully Get Appointment", statusCode: HttpStatusCodes.OK);
        }
    }
}
