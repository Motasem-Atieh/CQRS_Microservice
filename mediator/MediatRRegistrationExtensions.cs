
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class MediatRRegistrationExtensions
{
    public static IServiceCollection AddApplicationMediatR(this IServiceCollection services)
    {
        // Register all MediatR handlers from the current AppDomain assemblies
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });

        return services;
    }
}
