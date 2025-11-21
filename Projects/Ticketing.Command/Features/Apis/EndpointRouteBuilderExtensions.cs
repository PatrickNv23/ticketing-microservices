namespace Ticketing.Command.Features.Apis;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMinimalApisEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        // obtenemos todas las clases que están implementando IMinimalApi
        var minimalApis = endpointRouteBuilder.ServiceProvider.GetServices<IMinimalApi>();

        foreach (var minimalApi in minimalApis)
        {
            minimalApi.AddEndpoint(endpointRouteBuilder);
        }
        
        return endpointRouteBuilder;
    }
}