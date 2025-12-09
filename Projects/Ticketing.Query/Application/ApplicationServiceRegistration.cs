using System.Reflection;

namespace Ticketing.Query.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(m =>
        {
            m.RegisterServicesFromAssemblies(currentAssembly);
        });
        return services;
    }
}