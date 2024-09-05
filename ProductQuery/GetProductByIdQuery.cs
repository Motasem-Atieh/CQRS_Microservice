using CQRS_Microservice.Dto;
using MediatR;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}
