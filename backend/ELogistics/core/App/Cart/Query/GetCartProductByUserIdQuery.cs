using core.API_Response;
using core.Interface;
using domain.Model.Cart;
using domain.ModelDtos;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace core.App.Cart.Query
{
    public class GetCartProductByUserIdQuery : IRequest<AppResponse<List<CartProductDto>>>
    {
        public Guid CustomerId { get; set; }
    }
    public class GetCartProductByUserIdQueryHandler : IRequestHandler<GetCartProductByUserIdQuery, AppResponse<List<CartProductDto>>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetCartProductByUserIdQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<AppResponse<List<CartProductDto>>> Handle(GetCartProductByUserIdQuery request, CancellationToken cancellationToken)
        {
            // using entity framework linq to fetch cart products by customer id
            var products = await _appDbContext.Set<CartDetail>().Where(cd => cd.CartMaster.CustomerId == request.CustomerId).Select(cd => new CartProductDto
            {
                ProductId = cd.Product.Id,
                ProductName = cd.Product.ProductName,
                Quantity = cd.Quantity,
                Category = cd.Product.ProductCategory,
                ProductMRP = cd.Product.ProductMRP,
                ProductRate = cd.Product.ProductRate,
                SGST = GstSlab.GST_9,
                CGST = GstSlab.GST_9,
                ProductImageUrl = cd.Product.ProductImageUrl,
                TotalPrice = cd.Quantity * cd.Product.ProductRate,
            }).ToListAsync();

            if (products == null || !products.Any())
            {
                return AppResponse.Fail<List<CartProductDto>>(message: "No products found in the cart", statusCode: HttpStatusCodes.NotFound);
            }

            return AppResponse.Success(products, message: "Cart products fetched successfully", statusCode: HttpStatusCodes.OK);

        }
    }

}
