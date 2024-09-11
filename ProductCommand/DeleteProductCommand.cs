using System;
using CQRS_Microservice.Data;
using MediatR;

namespace CQRS_Microservice.ProductCommand;

public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }

        
    }

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
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