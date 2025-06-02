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
    public class GetAllSpecialisationQuery : IRequest<AppResponse<object>>
    {
    }

    public class GetAllSpecialisationQueryHandler : IRequestHandler<GetAllSpecialisationQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAllSpecialisationQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(GetAllSpecialisationQuery request, CancellationToken cancellationToken)
        {
            var connection = _appDbContext.GetConnection();
            var query = "SELECT * FROM Specialisations";
            var allSpecialisation = await connection.QueryAsync<domain.Model.User.Specialisation>(query);

            return AppResponse.Success<object>(allSpecialisation.ToList(), "Successfully Fetch All Specialisation", HttpStatusCodes.OK);
        }
    }
}
