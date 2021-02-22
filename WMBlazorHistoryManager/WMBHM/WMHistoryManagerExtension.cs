using Microsoft.Extensions.DependencyInjection;

public static class WMHistoryManagerExtension
{
    public static IServiceCollection AddWMHistoryManager(this IServiceCollection services)
    {
        return services.AddScoped<IWMHistoryManager>(p =>
        {
            var WMBH = ActivatorUtilities.CreateInstance<WMHistoryManagerCore>(p);
            return WMBH;
        });
    }
}