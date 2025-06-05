// using core.Interface;
// using Dapper;
// using domain.ModelDto;
// using Mapster;
// using MediatR;
// using Microsoft.Extensions.Configuration;
// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// 
// namespace core.App.Order.Query
// {
//     public class GetOrderByUserIdQuery : IRequest<List<domain.ModelDto.OrderDto>>
//     {
//         public int UserId { get; set; }
//     }
//     public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrderByUserIdQuery, List<domain.ModelDto.OrderDto>>
//     {
//         private readonly IAppDbContext _context;
//         public GetOrderByUserIdQueryHandler(core.Interface.IAppDbContext context)
//         {
//             _context = context;
//         }
//         public async Task<List<domain.ModelDto.OrderDto>> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
//         {
//             var connection = _context.GetConnection();
// 
//             var query = @"
//         SELECT sm.InvoiceId, sm.InvoiceDate, sm.Subtotal, sm.DeliveryAddress, 
//                sm.InvoicePdfLink, s.Name AS DeliveryState, c.Name AS DeliveryCountry
//         FROM SalesMasters sm
//         JOIN States s ON sm.DeliveryStateId = s.StateId
//         JOIN Countries c ON sm.DeliveryCountryId = c.CountryId
//         WHERE sm.UserId = @UserId;";
// 
//             var result = await connection.QueryAsync<domain.ModelDto.OrderDto>(query, new { UserId = request.UserId });
// 
// 
//             return result.ToList();
//         }
//     }
// 
// }
