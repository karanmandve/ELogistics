/*
// using core.API_Response;
// using core.Interface;
// using Dapper;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace App.Core.Apps.State.Query
{
    public class GetAllState : IRequest<AppResponse<object>>
    {
    }


    public class GetAllStateQueryHandler : IRequestHandler<GetAllState, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAllStateQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(GetAllState request, CancellationToken cancellationToken)
        {
            var connection = _appDbContext.GetConnection();
            var query = "SELECT * FROM States";
            var allState = await connection.QueryAsync<domain.Model.State.State>(query);

            return AppResponse.Success<object>(allState.ToList(), "Successfully Fetch All State", HttpStatusCodes.OK);
        }
    }







}
*/
