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
//
// namespace core.App.User.Query
// {
//     public class GetUserByUsernameQuery : IRequest<AppResponse<object>>
//     {
//         public string Username { get; set; }
//     }
//
//     public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByUsernameQuery, AppResponse<object>>
//     {
//         private readonly IAppDbContext _context;
//
//         public GetUserByEmailQueryHandler(IAppDbContext context)
//         {
//             _context = context;
//         }
//         public async Task<AppResponse<object>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
//         {
//             var username = request.Username;
//             var user = await _context.Set<domain.Model.User.User>().FirstOrDefaultAsync(x => x.Username == username);
//
//             if (user == null)
//             {
//                 return AppResponse.Fail<object>(message: "User Not Found", statusCode: HttpStatusCodes.NotFound);
//             }
//
//             var userDetail = user.Adapt<domain.ModelDto.UserDto>();
//
//             return AppResponse.Success<object>(userDetail, "Successfully Fetch User", HttpStatusCodes.OK);
//
//         }
//     }
// }
