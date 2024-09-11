
using System.ComponentModel.DataAnnotations;

namespace CQRS_Microservice.Models;
public class Product
{
public int Id { get; set; } 
[Required] [StringLength(20)] 
public string Name { get; set; }
    
[Required] [StringLength(50)] public string Description { get; set; }
public decimal Price { get; set; }
public DateTimeOffset  CreatedDate { get; set; }
}

    
