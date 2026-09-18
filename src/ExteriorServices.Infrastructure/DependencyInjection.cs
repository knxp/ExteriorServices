using ExteriorServices.Application.Customers;
using ExteriorServices.Application.Properties;
using ExteriorServices.Infrastructure.Data;
using ExteriorServices.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExteriorServices.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<Application.Dashboard.IDashboardRepository, DashboardRepository>();

        services.AddDbContext<ExteriorServicesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}