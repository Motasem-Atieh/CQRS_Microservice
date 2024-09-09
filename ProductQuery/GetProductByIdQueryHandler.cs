using AutoMapper;
using CQRS_Microservice.Data;
using CQRS_Microservice.Dto;

using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CQRS_Microservice.ProductQuery;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    // make the query and its handler in one class

    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        //
        var product = await _context.Products.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

        return _mapper.Map<ProductDto>(product);
    }
}