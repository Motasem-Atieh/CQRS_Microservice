using CQRS_Microservice.Data;
using CQRS_Microservice.Models;
using MediatR;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
{
    // make the command and its handler in one class

    private readonly ApplicationDbContext _context;

    public CreateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
