using System;
using MediatR;

namespace CQRS_Microservice.ProductCommand;

 public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
    }