// using core.Interface;
// using Dapper;
// using domain.ModelDto;
// using Mapster;
// using MediatR;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace core.App.Product.Query
// {
//     public class GetAllProductByUserIdQuery : IRequest<List<domain.Model.Products.Product>>
//     {
//         public int UserId { get; set; }
//     }

//     public class GetAllProductByUserIdQueryHandler : IRequestHandler<GetAllProductByUserIdQuery, List<domain.Model.Products.Product>>
//     {
//         private readonly IAppDbContext _context;
//         public GetAllProductByUserIdQueryHandler(core.Interface.IAppDbContext context)
//         {
//             _context = context;
//         }
//         public async Task<List<domain.Model.Products.Product>> Handle(GetAllProductByUserIdQuery request, System.Threading.CancellationToken cancellationToken)
//         {
//             using var connection = _context.GetConnection();

//             var query = "SELECT * FROM Products WHERE UserId = @UserId AND IsDeleted = 0;";

//             var data = await connection.QueryAsync<domain.Model.Products.Product>(query, new { UserId = request.UserId });


//             return data.ToList();
//         }
//     }


// }
