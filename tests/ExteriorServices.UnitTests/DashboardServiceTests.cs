using ExteriorServices.Application.Dashboard;
using ExteriorServices.Domain.Entities;
using Xunit;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_ReturnsCustomerAndPropertyCounts()
    {
        var repository = new InMemoryDashboardRepository();
        repository.Customers.Add(new Customer { Id = 1, IsActive = true });
        repository.Customers.Add(new Customer { Id = 2, IsActive = false });
        repository.Customers.Add(new Customer { Id = 3, IsActive = true });
        repository.Properties.Add(new Property { Id = 10, CustomerId = 1 });
        repository.Properties.Add(new Property { Id = 11, CustomerId = 1 });
        repository.Properties.Add(new Property { Id = 12, CustomerId = 3 });

        var service = new DashboardService(repository);

        var result = await service.GetDashboardAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCustomers);
        Assert.Equal(2, result.ActiveCustomers);
        Assert.Equal(3, result.TotalProperties);
    }

    private sealed class InMemoryDashboardRepository : IDashboardRepository
    {
        public List<Customer> Customers { get; } = new();

        public List<Property> Properties { get; } = new();

        public Task<int> GetTotalCustomersAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Customers.Count);
        }

        public Task<int> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Customers.Count(customer => customer.IsActive));
        }

        public Task<int> GetTotalPropertiesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Properties.Count);
        }
    }
}
