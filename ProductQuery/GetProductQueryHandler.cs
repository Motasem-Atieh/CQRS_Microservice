using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRS_Microservice.Data;
using CQRS_Microservice.Dto;
using AutoMapper;
using MediatR;
using CQRS_Microservice.Query;
using Microsoft.EntityFrameworkCore;

namespace CQRS_Microservice.Handlers
{
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
}
