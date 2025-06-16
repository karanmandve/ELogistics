using core.API_Response;
using core.Interface;
using domain.Model.Cart;
using domain.ModelDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.Cart.Command
{
    public class UpdateCartQuantityCommand : IRequest<AppResponse<object>>
    {
        public CartQuantityChangeDto QuantityChangeData { get; set; }
    }

    public class UpdateCartQuantityCommandHandler : IRequestHandler<UpdateCartQuantityCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public UpdateCartQuantityCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(UpdateCartQuantityCommand request, CancellationToken cancellationToken)
        {
            var quantityChangeData = request.QuantityChangeData;

            // Get the CartDetail for the user and product
            var cartDetail = await _appDbContext.Set<CartDetail>()
                .FirstOrDefaultAsync(cd => cd.CartMaster.CustomerId == quantityChangeData.CustomerId && cd.ProductId == quantityChangeData.ProductId);

            if (cartDetail == null)
            {
                return AppResponse.Fail<object>(message: "No such product in the user's cart", statusCode: HttpStatusCodes.NotFound);
            }

            // Get the product to update its stock
            var product = await _appDbContext.Set<domain.Model.Products.Product>()
                .FirstOrDefaultAsync(p => p.Id == quantityChangeData.ProductId);

            if (product == null)
            {
                return AppResponse.Fail<object>(message: "Product not found", statusCode: HttpStatusCodes.NotFound);
            }

            if (product.AvailableStocks < quantityChangeData.QuantityChange)
            {
                return AppResponse.Fail<object>(message: "Insufficient stock", statusCode: HttpStatusCodes.Conflict);
            }

            // Check if the quantity change is valid
            if (quantityChangeData.QuantityChange == 1)
            {
                // Increment: Update Cart Detail and Product stock
                cartDetail.Quantity += 1;
                product.AvailableStocks -= 1;
            }
            else if (quantityChangeData.QuantityChange == -1)
            {
                // Decrement: Update Cart Detail and Product stock
                if (cartDetail.Quantity > 1)
                {
                    cartDetail.Quantity -= 1;
                    product.AvailableStocks += 1;
                }
                else if (cartDetail.Quantity == 1)
                {
                    // If quantity is 1, remove the item from CartDetail
                    _appDbContext.Set<CartDetail>().Remove(cartDetail);
                    product.AvailableStocks += 1; // Restore the stock as the product is removed from the cart
                }
            }
            else
            {
                return AppResponse.Fail<object>(message: "Invalid quantity change", statusCode: HttpStatusCodes.BadRequest);
            }

            // Save the changes
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return AppResponse.Success<object>(message: "Cart quantity updated successfully", statusCode: HttpStatusCodes.OK);
        }
    }
}