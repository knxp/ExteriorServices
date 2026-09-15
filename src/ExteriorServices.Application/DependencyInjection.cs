using Microsoft.Extensions.DependencyInjection;

namespace ExteriorServices.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<Customers.ICustomerService, Customers.CustomerService>();
        services.AddScoped<Properties.IPropertyService, Properties.PropertyService>();

        return services;
    }
}