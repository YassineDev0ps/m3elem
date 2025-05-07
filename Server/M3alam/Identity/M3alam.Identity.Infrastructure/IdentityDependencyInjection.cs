
using M3alam.Common.Interfaces;
using M3alam.Identity.Infrastructure.Context;
using M3alam.Identity.Infrastructure.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;






namespace SchoolSaas.Infrastructure.Backoffice
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
         
            services.AddTransient<IIdentityService, IdentityService>();
           


            return services;
        }
    }
}