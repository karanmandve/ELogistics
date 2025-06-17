using core.API_Response;
using core.Interface;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using domain.Model.Users;
using domain.ModelDtos;

namespace core.App.User.Query
{
    public class GetUserByEmailQuery : IRequest<AppResponse<object>>
    {
        public string Email { get; set; }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _context;

        public GetUserByEmailQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AppResponse<object>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var email = request.Email;
            var customer = await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
            if (customer != null)
            {
                var userDetail = customer.Adapt<UserDto>();
                return AppResponse.Success<object>(userDetail, "Successfully fetched user (customer)", HttpStatusCodes.OK);
            }
            var distributor = await _context.Set<Distributor>().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
            if (distributor != null)
            {
                var userDetail = distributor.Adapt<UserDto>();
                return AppResponse.Success<object>(userDetail, "Successfully fetched user (distributor)", HttpStatusCodes.OK);
            }
            return AppResponse.Fail<object>(message: "User Not Found", statusCode: HttpStatusCodes.NotFound);
        }
    }
}
