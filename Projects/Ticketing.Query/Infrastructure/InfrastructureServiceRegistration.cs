using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infrastructure.Consumers;
using Ticketing.Query.Infrastructure.Persistence;
using Ticketing.Query.Infrastructure.Repositories;

namespace Ticketing.Query.Infrastructure;

public static class InfrastructureServiceRegistration
{

    public static IServiceCollection RegisterInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        Action<DbContextOptionsBuilder> configureDbContext;

        var connectionString = configuration
                                   .GetConnectionString("PostgresConnectionString")
                               ?? throw new ArgumentException(nameof(configuration));

        configureDbContext = o => o.UseLazyLoadingProxies()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
     
        //services.AddDbContext<TicketDbContext>(configureDbContext);

        services.AddDbContext<TicketDbContext>(opt => {
        
            opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddSingleton<DatabaseContextFactory>(
            new DatabaseContextFactory(configureDbContext)
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(
            typeof(IGenericRepository<>), 
            typeof(GenericRepository<>)
        );

   

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddHostedService<ConsumerHostedService>();
        services.AddScoped<IEventHandler, Handlers.EventHandler>();
   
        services.Configure<ConsumerConfig>(
            configuration.GetSection(nameof(ConsumerConfig))
        );

        return services;
    }

}
