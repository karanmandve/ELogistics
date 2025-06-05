// using core.API_Response;
// using core.Interface;
// using Dapper;
// using MediatR;
// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace core.App.Cart.Query
// {
//     public class CartCountQuery : IRequest<AppResponse<object>>
//     {
//         public int UserId { get; set; }
//     }

//     public class CartCountQueryHandler : IRequestHandler<CartCountQuery, AppResponse<object>>
//     {
//         private readonly IAppDbContext _appDbContext;

//         public CartCountQueryHandler(IAppDbContext appDbContext)
//         {
//             _appDbContext = appDbContext;
//         }

//         public async Task<AppResponse<object>> Handle(CartCountQuery request, CancellationToken cancellationToken)
//         {
//         }
//     }
// }


