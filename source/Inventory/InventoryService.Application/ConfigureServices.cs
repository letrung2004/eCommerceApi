using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InventoryService.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                //ctg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            return services;
        }
    }
}
