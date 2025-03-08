namespace Backoffice.Client.ServiceProxies;

public static class ProxiesDependencyRegistrar
{
    public static IServiceCollection AddServiceProxies(this IServiceCollection services)
    {
        services.AddScoped<Commands.Administrators.CreateDefaultAdministratorCommandProxy>();
        
        return services;
    }
}