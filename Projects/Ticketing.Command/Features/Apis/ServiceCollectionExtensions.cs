using System.Reflection;

namespace Ticketing.Command.Features.Apis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMinimalApis(this IServiceCollection services)
    {
        // obtener todas las clases que implementan minimalApis según criterios
        var currentAssembly = Assembly.GetExecutingAssembly();
        var minimalApis = currentAssembly.GetTypes()
            .Where(t => typeof(IMinimalApi).IsAssignableFrom(t)
                        && t != typeof(IMinimalApi)
                        && t.IsPublic 
                        && !t.IsAbstract
            );
        
        // registrar a todas las minimalApis como servicios en el contenedor
        foreach (var minimalApi in minimalApis)
        {
            services.AddSingleton(typeof(IMinimalApi), minimalApi);
        }
        return services;
    }
}