using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class MediatRRegistrationExtensions
{
    public static IServiceCollection AddApplicationMediatR(this IServiceCollection services)
    {
        // Register MediatR services and handlers from a specific assembly
        services.AddMediatR(cfg =>
        {
            // Specify the assembly where your MediatR handlers are located
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
