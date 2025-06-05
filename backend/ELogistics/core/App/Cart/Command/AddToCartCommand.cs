using core.Interface;
using domain.ModelDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using core.API_Response;

namespace core.App.Cart.Command
{
    public class AddToCartCommand : IRequest<AppResponse<object>>
    {
        public AddToCartDto AddToCartData { get; set; }
    }

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public AddToCartCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<AppResponse<object>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                var addToCartData = request.AddToCartData;

                // Fetch CartMaster
                var cartMasterData = await _context.Set<domain.Model.Cart.CartMaster>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cart => cart.CustomerId == addToCartData.CustomerId, cancellationToken);

                // use for creating a new cart if it doesn't exist
                if (cartMasterData == null)
                {
                    cartMasterData = new domain.Model.Cart.CartMaster
                    {
                        CustomerId = addToCartData.CustomerId
                    };

                    await _context.Set<domain.Model.Cart.CartMaster>().AddAsync(cartMasterData, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Fetch Product
                var product = await _context.Set<domain.Model.Products.Product>()
                    .FirstOrDefaultAsync(p => p.Id == addToCartData.ProductId, cancellationToken);

                if (product == null || product.AvailableStocks < addToCartData.Quantity)
                {
                    return AppResponse.Fail<object>(message: "Product not available or insufficient stock", statusCode: HttpStatusCodes.Conflict);
                }

                // Fetch existing CartDetail
                var existingCartDetail = await _context.Set<domain.Model.Cart.CartDetail>()
                    .FirstOrDefaultAsync(cd => cd.CartId == cartMasterData.Id && cd.ProductId == addToCartData.ProductId, cancellationToken);

                if (existingCartDetail != null)
                {
                    existingCartDetail.Quantity += addToCartData.Quantity;
                    _context.Set<domain.Model.Cart.CartDetail>().Update(existingCartDetail);
                }
                else
                {
                    var cartDetail = new domain.Model.Cart.CartDetail
                    {
                        CartId = cartMasterData.Id,
                        ProductId = addToCartData.ProductId,
                        Quantity = addToCartData.Quantity
                    };
                    await _context.Set<domain.Model.Cart.CartDetail>().AddAsync(cartDetail, cancellationToken);
                }

                // Update product stock
                product.AvailableStocks -= addToCartData.Quantity;
                _context.Set<domain.Model.Products.Product>().Update(product);

                await _context.SaveChangesAsync(cancellationToken);

                return AppResponse.Success<object>(message: "Product added to cart successfully", statusCode: HttpStatusCodes.OK);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
