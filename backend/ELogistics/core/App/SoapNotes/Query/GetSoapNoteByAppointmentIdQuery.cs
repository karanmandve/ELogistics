using core.API_Response;
using core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.SoapNotes.Query
{
    public class GetSoapNoteByAppointmentIdQuery : IRequest<AppResponse<object>>
    {
        public int AppointmentId { get; set; }
    }
    //i want get soap note by appointment id commandhandler pleaes give copilot
    public class GetSoapNoteByAppointmentIdQueryHandler : IRequestHandler<GetSoapNoteByAppointmentIdQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        public GetSoapNoteByAppointmentIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<AppResponse<object>> Handle(GetSoapNoteByAppointmentIdQuery request, CancellationToken cancellationToken)
        {

            var soapNote = await _context.Set<domain.Model.SoapNotes.SoapNote>().FirstOrDefaultAsync(x => x.AppointmentId == request.AppointmentId);
            if (soapNote == null)
            {
                return AppResponse.Fail<object>(message: "Soap Note Not Found", statusCode: HttpStatusCodes.NotFound);
            }
            return AppResponse.Success<object>(soapNote, "Soap Note Fetch Successfully", HttpStatusCodes.OK);

        }
    }
}
