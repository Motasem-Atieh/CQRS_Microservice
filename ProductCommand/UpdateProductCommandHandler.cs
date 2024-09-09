using CQRS_Microservice.Data;
using MediatR;
namespace CQRS_Microservice.ProductCommand;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    // make the command and its handler in one class

    private readonly ApplicationDbContext _context;

    public UpdateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        //get the product from cache
        var product = await _context.Products.FindAsync(request.Id);

        if (product == null) return false;

        product.Name = request.Name;
        product.Price = request.Price;
        product.Description = request.Description;

        _context.Products.Update(product);

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }

}
