using System;
using AutoMapper;
using CQRS_Microservice.Data;
using CQRS_Microservice.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS_Microservice.Query;

public class GetProductQuery :IRequest<IEnumerable<ProductDto>>
{

}

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, IEnumerable<ProductDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products.ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
