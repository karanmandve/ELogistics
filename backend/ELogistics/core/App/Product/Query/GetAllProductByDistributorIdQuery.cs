using core.API_Response;
using core.Interface;
using domain.ModelDtos;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace core.App.Product.Query
{
    public class GetAllProductByDistributorIdQuery : IRequest<AppResponse<object>>
    {
        public Guid DistributorId { get; set; }
    }

    public class GetAllProductByDistributorIdQueryHandler : IRequestHandler<GetAllProductByDistributorIdQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        public GetAllProductByDistributorIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AppResponse<object>> Handle(GetAllProductByDistributorIdQuery request, CancellationToken cancellationToken)
        {
            // use entity framework to get all products by distributor id
            var products = await _context.Set<domain.Model.Products.Product>()
                .Where(x => x.DistributorId == request.DistributorId).ToListAsync(cancellationToken);

            if (products == null || !products.Any())
            {
                return AppResponse.Fail<object>(message: "No products found for this distributor", statusCode: HttpStatusCodes.NotFound);
            }

            var allProducts = products.Adapt<List<AllProductResponseDto>>();
            
            return AppResponse.Success<object>(data: allProducts, message: "Products retrieved successfully", statusCode: HttpStatusCodes.OK);
        }
    }

} 