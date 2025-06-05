// using App.Core.Apps.Country.Query;
// using core.App.Order.Query;
// using MediatR;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;

// namespace EComApplication.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class OrderController : ControllerBase
//     {
//         private readonly IMediator _mediator;

//         public OrderController(IMediator mediator)
//         {
//             _mediator = mediator;
//         }

//         [HttpGet("get-orders-by-userId/{userId}")]
//         public async Task<IActionResult> GetOrdersByUserId(int userId)
//         {
//             var allOrders = await _mediator.Send(new GetOrderByUserIdQuery { UserId = userId });
//             return Ok(allOrders);
//         }

//     }
// }
