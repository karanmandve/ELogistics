using core.App.Cart.Command;
using core.App.Cart.Query;
using domain.ModelDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EComApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(AddToCartDto model)
        {
            var command = new AddToCartCommand
            {
                AddToCartData = model
            };
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return Conflict(result);
            }
            return Ok(result);
        }

        [HttpGet("get-cart-products/{customerId}")]
        public async Task<IActionResult> GetCartProducts(Guid customerId)
        {
            var query = new GetCartProductByUserIdQuery { CustomerId = customerId };
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPut("update-cart-quantity")]
        public async Task<IActionResult> UpdateCartQuantity(domain.ModelDto.CartQuantityChangeDto quantityChangeData)
        {
            if (quantityChangeData == null || (quantityChangeData.QuantityChange != 1 && quantityChangeData.QuantityChange != -1))
            {
                return BadRequest("Invalid quantity change.");
            }
            var command = new UpdateCartQuantityCommand
            {
                QuantityChangeData = quantityChangeData
            };
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // [HttpGet("get-cart-count/{userId}")]
        // public async Task<IActionResult> GetCartCount(int userId)
        // {
        //     var result = await _mediator.Send(new CartCountQuery { UserId = userId });
        //     if (!result.IsSuccess)
        //     {
        //         return NotFound(result);
        //     }
        //     return Ok(result);
        // }

        [HttpDelete("remove-product-from-cart/{productId}/{customerId}")]
        public async Task<IActionResult> RemoveProductById(Guid productId, Guid customerId)
        {
            var result = await _mediator.Send(new RemoveProductFromCartById { ProductId = productId, CustomerId = customerId });

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }



        [HttpPost("checkout/{customerId}")]
        public async Task<IActionResult> Checkout(Guid customerId)
        {
            var command = new CheckoutCommand
            {
                CustomerId = customerId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
