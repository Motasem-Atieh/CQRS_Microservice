using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS_Microservice.ProductCommand;
using CQRS_Microservice.Data;
using CQRS_Microservice.Models; 
using MediatR;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
{
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
