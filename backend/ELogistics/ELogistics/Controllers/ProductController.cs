using core.App.Product.Command;
using domain.ModelDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using core.API_Response;
using core.App.Product.Query;

namespace EComApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(ProductDto model)
        {
            if (model.ImageFile == null)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Image is required"
                });
            }

            using (var stream = model.ImageFile.OpenReadStream())
            {
                var command = new AddProductCommand
                {
                    FileStream = stream,
                    FileName = model.ImageFile.FileName,
                    Product = model
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return Conflict(result);
                }
                return Ok(result);
            }
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { ProductId = id });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductDto product)
        {
            if (product.ImageFile == null)
            {
                var command = new UpdateProductCommand
                {
                    FileStream = null,
                    FileName = null,
                    Product = product
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }

            using (var stream = product.ImageFile.OpenReadStream())
            {
                var command = new UpdateProductCommand
                {
                    FileStream = stream,
                    FileName = product.ImageFile.FileName,
                    Product = product
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
        }

        [HttpGet("get-product-by-distributor/{distributorId}")]
        public async Task<IActionResult> GetProductByDistributorId(Guid distributorId)
        {
            var result = await _mediator.Send(new GetAllProductByDistributorIdQuery { DistributorId = distributorId });

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        //         [HttpGet("get-all-product")]
        //         public async Task<IActionResult> GetAllProduct()
        //         {
        //             var result = await _mediator.Send(new GetAllProductQuery { });

        //             if (result == null)
        //             {
        //                 return NotFound(new
        //                 {
        //                     statusCode = 404,
        //                     message = "Product not found"
        //                 });
        //             }

        //             return Ok(result);
        //         }

    }
}
