// using core.Interface;
// using Dapper;
// using MediatR;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace core.App.Product.Query
// {
//     public class GetAllProductQuery : IRequest<List<domain.Model.Products.Product>>
//     {
//     }
//     public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<domain.Model.Products.Product>>
//     {
//         private readonly IAppDbContext _context;
//         public GetAllProductQueryHandler(core.Interface.IAppDbContext context)
//         {
//             _context = context;
//         }
//         public async Task<List<domain.Model.Products.Product>> Handle(GetAllProductQuery request, System.Threading.CancellationToken cancellationToken)
//         {
//             using var connection = _context.GetConnection();

//             var query = "SELECT * FROM Products WHERE IsDeleted = 0 AND Stock > 0;";

//             var data = await connection.QueryAsync<domain.Model.Products.Product>(query);

//             return data.ToList();
//         }
//     }
// }
