namespace FieldServiceManagement.Business.Infrastructure;
public static class ServiceProviderAccessor
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static void Initialise(IServiceProvider services) =>
        Services = services ?? throw new ArgumentNullException(nameof(services));
}