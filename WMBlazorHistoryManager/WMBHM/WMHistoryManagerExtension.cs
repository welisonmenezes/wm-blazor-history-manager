using Microsoft.Extensions.DependencyInjection;

public static class WMHistoryManagerExtension
{
    public static IServiceCollection AddWMHistoryManager(this IServiceCollection services, bool useBrowserNativeBehavior = false)
    {
        return services.AddScoped<IWMHistoryManager>(p =>
        {
            var WMBH = ActivatorUtilities.CreateInstance<WMHistoryManagerCore>(p);
            WMBH.Configure(useBrowserNativeBehavior);
            return WMBH;
        });
    }
}