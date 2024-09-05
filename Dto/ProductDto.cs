using System;
using System.ComponentModel.DataAnnotations;

namespace CQRS_Microservice.Dto;

 public class ProductDto
    {
        public int Id { get; set; }
        // error desc
        [Required] [StringLength(20)] public  string Name { get; set; }
        //length 50
        [Required] [StringLength(20)] public string Description { get; set; }
        // range
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }