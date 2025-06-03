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
    public class GetAllProviderBySpecialisationIdQuery : IRequest<AppResponse<object>>
    {
        public int SpecialisationId { get; set; }
    }

    public class GetAllProviderBySpecialisationIdQueryHandler : IRequestHandler<GetAllProviderBySpecialisationIdQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _context;

        public GetAllProviderBySpecialisationIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AppResponse<object>> Handle(GetAllProviderBySpecialisationIdQuery request, CancellationToken cancellationToken)
        {
            var specialisationId = request.SpecialisationId;
            var providers = await _context.Set<domain.Model.User.User>().Where(x => x.SpecialisationId == specialisationId && x.UserTypeId == 1).ToListAsync();
            // it is empty array so how to handle it below

            if (!providers.Any())
            {
                return AppResponse.Fail<object>(message: "Providers Not Found", statusCode: HttpStatusCodes.NotFound);
            }

            var providersDetail = providers.Adapt<List<domain.ModelDto.UserDto>>();

            return AppResponse.Success<object>(providersDetail, "Successfully Fetch Providers", HttpStatusCodes.OK);

        }
    }

}
*/
