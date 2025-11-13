using AutoMapper;
using SharedLibrarySolution.Interfaces;
using System.Reflection;

namespace SharedLibrarySolution.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a =>
                            !a.FullName!.StartsWith("MassTransit") &&
                            !a.FullName!.StartsWith("RabbitMQ") &&
                            !a.IsDynamic &&
                            !string.IsNullOrEmpty(a.Location))
                        .ToList();

            foreach (var assembly in assemblies)
            {
                try
                {
                    ApplyMappingsFromAssembly(assembly);
                }
                catch (ReflectionTypeLoadException)
                {
                    // Bỏ qua các assembly không load được type
                    continue;
                }
            }

        }
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var mapFromType = typeof(IMapFrom<>);

            var mappingMethodName = nameof(IMapFrom<object>.Mapping);

            bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

            var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

            var argumentTypes = new Type[] { typeof(Profile) };

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(mappingMethodName);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(instance, new object[] { this });
                }
                else
                {
                    var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                    if (interfaces.Count > 0)
                    {
                        foreach (var @interface in interfaces)
                        {
                            var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                            interfaceMethodInfo?.Invoke(instance, new object[] { this });
                        }
                    }
                }
            }
        }
    }
}
