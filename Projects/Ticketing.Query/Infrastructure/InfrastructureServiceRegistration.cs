using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Persistence;
using Ticketing.Query.Infrastructure.Repositories;

namespace Ticketing.Query.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection RegisterInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Action<DbContextOptionsBuilder> configureDbContext;

        var connectionString = configuration
            .GetConnectionString("PostgresConnectionString")
            ?? throw new ArgumentNullException(nameof(configuration));
        
        configureDbContext = options => options
            .UseLazyLoadingProxies()
            .UseNpgsql(connectionString) // conexión lazy loading
            .UseSnakeCaseNamingConvention(); // que se cree con la nomenclatura de snake

        // services.AddDbContext<TicketDbContext>(configureDbContext);
        services.AddDbContext<TicketDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        services.AddSingleton(new DatabaseContextFactory(configureDbContext));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));
        
        return services;
    }
}