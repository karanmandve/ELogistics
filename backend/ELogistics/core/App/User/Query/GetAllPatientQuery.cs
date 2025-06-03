/*
// using core.API_Response;
// using core.Interface;
// using Mapster;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

namespace core.App.User.Query
{
    public class GetAllPatientQuery : IRequest<AppResponse<object>>
    {
    }
    //i want get all patient queryhandler pleaes give copilot
    public class GetAllPatientQueryHandler : IRequestHandler<GetAllPatientQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        public GetAllPatientQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<AppResponse<object>> Handle(GetAllPatientQuery request, CancellationToken cancellationToken)
        {
            var patients = await _context.Set<domain.Model.User.User>().Where(x => x.UserTypeId == 2).ToListAsync();

            var patientsData = patients.Adapt<List<domain.ModelDto.UserDto>>();

            if (patients == null)
            {
                return AppResponse.Fail<object>(message: "Patient Not Found", statusCode: HttpStatusCodes.NotFound);
            }
            return AppResponse.Success<object>(patientsData, "Patient Fetch Successfully", HttpStatusCodes.OK);

        }
    }
}
*/
