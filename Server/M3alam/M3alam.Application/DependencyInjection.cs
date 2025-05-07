// M3alam.Application/DependencyInjection.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace M3alam.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR and handlers from the current assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
