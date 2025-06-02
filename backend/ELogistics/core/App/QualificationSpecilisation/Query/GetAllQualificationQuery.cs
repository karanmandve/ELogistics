using core.API_Response;
using core.Interface;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.QualificationSpecilisation.Query
{
    public class GetAllQualificationQuery : IRequest<AppResponse<object>>
    {
    }

    public class GetAllQualificationQueryHandler : IRequestHandler<GetAllQualificationQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAllQualificationQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(GetAllQualificationQuery request, CancellationToken cancellationToken)
        {
            var connection = _appDbContext.GetConnection();
            var query = "SELECT * FROM Qualifications";
            var allQualification = await connection.QueryAsync<domain.Model.User.Qualification>(query);

            return AppResponse.Success<object>(allQualification.ToList(), "Successfully Fetch All Qualification", HttpStatusCodes.OK);
        }
    }
}
