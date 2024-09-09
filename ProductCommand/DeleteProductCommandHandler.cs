using CQRS_Microservice.Data;
using MediatR;

namespace CQRS_Microservice.ProductCommand;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    // make the command and its handler in one class

    private readonly ApplicationDbContext _context;
    public DeleteProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null) return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
