using Ticketing.Query.Application.Extensions;
using Ticketing.Query.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.RegisterInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.MapControllers();
await app.ApplyMigration(); // al ejecutar la aplicación aplicará las migraciones pendientes de la bd.
app.Run();
