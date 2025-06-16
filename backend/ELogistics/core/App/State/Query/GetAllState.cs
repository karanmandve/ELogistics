using core.API_Response;
using core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var allState = await _appDbContext.Set<domain.Model.States.State>().ToListAsync(cancellationToken);
            return AppResponse.Success<object>(allState, "Successfully Fetch All State", HttpStatusCodes.OK);
        }
    }
}
