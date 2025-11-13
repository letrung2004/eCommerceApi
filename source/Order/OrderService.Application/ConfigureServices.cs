using Microsoft.Extensions.DependencyInjection;
using SharedLibrarySolution.Mapping;
using System.Reflection;

namespace OrderService.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddAutoMapper(Assembly.GetExecutingAssembly()); // cũ cấu hình trong project thôi
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // mới cấu hình ở sharelibrary
            // fix lấy các assemby k lỗi

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            //.Where(a =>
            //    !a.FullName!.StartsWith("RabbitMQ.Client") &&
            //    !a.IsDynamic &&
            //    !string.IsNullOrEmpty(a.Location)) // tránh các assembly không có file thực
            //.ToArray();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
