

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;






namespace M3alam.Infrastructure
{
    public static class M3alamDependencyInjection
    {
        public static IServiceCollection AddM3alamInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
         
            //services.AddTransient< , >();
           


            return services;
        }
    }
}