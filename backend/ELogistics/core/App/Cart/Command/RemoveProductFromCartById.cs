using core.API_Response;
using core.Interface;
using Dapper;
using domain.Model.Cart;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace core.App.Cart.Command
{
    public class RemoveProductFromCartById : IRequest<AppResponse<object>>
    {
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class RemoveProductFromCartByIdHandler : IRequestHandler<RemoveProductFromCartById, AppResponse<object>>
    {
        private readonly IAppDbContext _appDbContext;

        public RemoveProductFromCartByIdHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<object>> Handle(RemoveProductFromCartById request, CancellationToken cancellationToken)
        {

            var cartDetailsData = await _appDbContext.Set<CartDetail>().Include(cd => cd.Product).FirstOrDefaultAsync(cd => cd.CartMaster.CustomerId == request.CustomerId && cd.ProductId == request.ProductId, cancellationToken);

            if (cartDetailsData == null)
            {
                return AppResponse.Fail<object>(message: "No such product in the user's cart", statusCode: HttpStatusCodes.NotFound);
            }

            cartDetailsData.Product.AvailableStocks += cartDetailsData.Quantity;
            _appDbContext.Set<domain.Model.Products.Product>().Update(cartDetailsData.Product);
            await _appDbContext.SaveChangesAsync();

            var toDelete = await _appDbContext.Set<CartDetail>().Where(cd =>
                    cd.CartMaster.CustomerId == request.CustomerId &&
                    cd.ProductId == request.ProductId
                )
                .ToListAsync(cancellationToken);

            if (toDelete == null)
            {
                return AppResponse.Fail<object>(message: "No such product in the user's cart", statusCode: HttpStatusCodes.NotFound);
            }

            _appDbContext.Set<CartDetail>().RemoveRange(toDelete);

            await _appDbContext.SaveChangesAsync(cancellationToken);

            return AppResponse.Success<object>(message: "Product removed from cart successfully", statusCode: HttpStatusCodes.OK);
        }
    }

}
