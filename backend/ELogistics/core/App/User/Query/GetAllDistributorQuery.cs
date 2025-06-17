using core.API_Response;
using core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using domain.Model.Users;
using domain.ModelDtos;

namespace core.App.User.Query
{
    public class GetAllDistributorQuery : IRequest<AppResponse<List<DistributorDto>>>
    {
    }

    public class GetAllDistributorQueryHandler : IRequestHandler<GetAllDistributorQuery, AppResponse<List<DistributorDto>>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAllDistributorQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<List<DistributorDto>>> Handle(GetAllDistributorQuery request, CancellationToken cancellationToken)
        {
            var distributors = await _appDbContext.Set<Distributor>()
                .Select(d => new DistributorDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName
                })
                .ToListAsync(cancellationToken);

            return AppResponse.Success<List<DistributorDto>>(distributors, "Successfully fetched all distributors", HttpStatusCodes.OK);
        }
    }
}
