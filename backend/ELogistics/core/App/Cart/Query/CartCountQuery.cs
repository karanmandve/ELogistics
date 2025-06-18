using core.API_Response;
using core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using domain.Model.Cart;

namespace core.App.Cart.Query
{
    public class CartCountQuery : IRequest<AppResponse<object>>
    {
        public Guid CustomerId { get; set; }
    }

    public class CartCountQueryHandler : IRequestHandler<CartCountQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public CartCountQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(CartCountQuery request, CancellationToken cancellationToken)
        {
            // Find the CartMaster for the given user
            var cart = await _appDbContext.Set<CartMaster>()
                .Include(cm => cm.CartDetails)
                .FirstOrDefaultAsync(cm => cm.CustomerId == request.CustomerId, cancellationToken);

            if (cart == null)
            {
                return AppResponse.Success<object>(0, "Cart is empty", HttpStatusCodes.OK);
            }

            // Count distinct products in the cart
            var distinctProductCount = cart.CartDetails
                .Select(cd => cd.ProductId)
                .Distinct()
                .Count();

            return AppResponse.Success<object>(distinctProductCount, "Successfully fetched distinct product count", HttpStatusCodes.OK);
        }
    }
}


