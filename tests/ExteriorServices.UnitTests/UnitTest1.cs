using ExteriorServices.Application.Customers;
using ExteriorServices.Domain.Entities;
using Xunit;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateCustomerAsync_CreatesActiveCustomerAndReturnsDto()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        var result = await service.CreateCustomerAsync(new CreateCustomerRequest
        {
            FirstName = " Jane ",
            LastName = " Doe ",
            Phone = "555-0100",
            Email = "jane@example.com"
        });

        Assert.Equal(1, result.Id);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.True(result.IsActive);
        Assert.Single(repository.Customers);
    }

    [Fact]
    public async Task GetCustomerAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var service = new CustomerService(new InMemoryCustomerRepository());

        var result = await service.GetCustomerAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCustomerAsync_UpdatesCustomerAndReturnsDto()
    {
        var repository = new InMemoryCustomerRepository();
        repository.Customers.Add(new Customer
        {
            Id = 7,
            FirstName = "Old",
            LastName = "Name",
            IsActive = true
        });
        var service = new CustomerService(repository);

        var result = await service.UpdateCustomerAsync(7, new UpdateCustomerRequest
        {
            FirstName = "New",
            LastName = "Name",
            Email = "new@example.com",
            IsActive = true
        });

        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
        Assert.Equal("new@example.com", result.Email);
        Assert.Equal("New", repository.Customers[0].FirstName);
    }

    [Fact]
    public async Task DeactivateCustomerAsync_SoftDeletesCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        repository.Customers.Add(new Customer
        {
            Id = 3,
            FirstName = "Active",
            LastName = "Customer",
            IsActive = true
        });
        var service = new CustomerService(repository);

        var result = await service.DeactivateCustomerAsync(3);

        Assert.True(result);
        Assert.False(repository.Customers[0].IsActive);
        Assert.Single(repository.Customers);
    }

    private sealed class InMemoryCustomerRepository : ICustomerRepository
    {
        public List<Customer> Customers { get; } = new();

        public Task<IReadOnlyList<Customer>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Customer>>(Customers.ToList());
        }

        public Task<Customer?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Customers.FirstOrDefault(customer => customer.Id == id));
        }

        public Task AddAsync(
            Customer customer,
            CancellationToken cancellationToken = default)
        {
            customer.Id = Customers.Count == 0 ? 1 : Customers.Max(existing => existing.Id) + 1;
            Customers.Add(customer);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
