using System;
using MediatR;

namespace CQRS_Microservice.ProductCommand;

public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }

        
    }

