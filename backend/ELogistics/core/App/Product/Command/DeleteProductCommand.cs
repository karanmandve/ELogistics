using core.Interface;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using core.API_Response;

namespace core.App.Product.Command
{
    public class DeleteProductCommand : IRequest<AppResponse<object>>
    {
        public Guid ProductId { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        public DeleteProductCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AppResponse<object>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Set<domain.Model.Products.Product>().FindAsync(request.ProductId);
            if (product == null)
            {
                return AppResponse.Fail<object>(message: "Product not found", statusCode: HttpStatusCodes.NotFound);
            }
            product.IsActive = false;
            product.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return AppResponse.Success<object>(null, "Product Deleted Successfully", HttpStatusCodes.OK);
        }
    }
}
