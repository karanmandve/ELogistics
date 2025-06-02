
using core.API_Response;
using core.Interface;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace App.Core.Apps.Country.Query
{
    public class GetAllCountryQuery : IRequest<AppResponse<object>>
    {
    }

    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAllCountryQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            var connection = _appDbContext.GetConnection();
            var query = "SELECT * FROM Countries";
            var allCountry = await connection.QueryAsync<domain.Model.CountryState.Country>(query);

            return AppResponse.Success<object>(allCountry.ToList(), "Successfully Fetch All Country", HttpStatusCodes.OK);
        }
    }


}
