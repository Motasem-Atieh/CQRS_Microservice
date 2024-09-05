using System;
using CQRS_Microservice.Dto;
using MediatR;

namespace CQRS_Microservice.Query;

public class GetProductQuery :IRequest<IEnumerable<ProductDto>>
{

}

