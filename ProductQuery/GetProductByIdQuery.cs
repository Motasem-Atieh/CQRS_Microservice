using AutoMapper;
using CQRS_Microservice.Data;
using CQRS_Microservice.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}


public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

        return _mapper.Map<ProductDto>(product);
    }
}
