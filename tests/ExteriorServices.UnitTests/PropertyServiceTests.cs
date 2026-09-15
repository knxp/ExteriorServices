using ExteriorServices.Application.Properties;
using ExteriorServices.Domain.Entities;
using Xunit;

public class PropertyServiceTests
{
    [Fact]
    public async Task CreatePropertyAsync_WhenCustomerExists_CreatesProperty()
    {
        var repository = new InMemoryPropertyRepository();
        repository.Customers.Add(new Customer { Id = 5 });
        var service = new PropertyService(repository);

        var result = await service.CreatePropertyAsync(5, new CreatePropertyRequest
        {
            AddressLine1 = " 123 Main Street ",
            City = " Raleigh ",
            State = " NC ",
            PostalCode = "27601"
        });

        Assert.NotNull(result);
        Assert.Equal(5, result.CustomerId);
        Assert.Equal("123 Main Street", result.AddressLine1);
        Assert.Equal("Raleigh", result.City);
        Assert.Single(repository.Properties);
    }

    [Fact]
    public async Task CreatePropertyAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var repository = new InMemoryPropertyRepository();
        var service = new PropertyService(repository);

        var result = await service.CreatePropertyAsync(99, new CreatePropertyRequest
        {
            AddressLine1 = "123 Main Street",
            City = "Raleigh",
            State = "NC",
            PostalCode = "27601"
        });

        Assert.Null(result);
        Assert.Empty(repository.Properties);
    }

    [Fact]
    public async Task GetCustomerPropertiesAsync_ReturnsOnlyPropertiesForCustomer()
    {
        var repository = new InMemoryPropertyRepository();
        repository.Properties.AddRange(new[]
        {
            new Property { Id = 1, CustomerId = 7, AddressLine1 = "One" },
            new Property { Id = 2, CustomerId = 8, AddressLine1 = "Two" }
        });
        var service = new PropertyService(repository);

        var result = await service.GetCustomerPropertiesAsync(7);

        var property = Assert.Single(result);
        Assert.Equal(1, property.Id);
        Assert.Equal(7, property.CustomerId);
    }

    [Fact]
    public async Task UpdatePropertyAsync_WhenPropertyExists_UpdatesAddress()
    {
        var repository = new InMemoryPropertyRepository();
        repository.Properties.Add(new Property
        {
            Id = 3,
            CustomerId = 7,
            AddressLine1 = "Old Address",
            City = "Raleigh",
            State = "NC",
            PostalCode = "27601"
        });
        var service = new PropertyService(repository);

        var result = await service.UpdatePropertyAsync(3, new UpdatePropertyRequest
        {
            AddressLine1 = "New Address",
            City = "Durham",
            State = "NC",
            PostalCode = "27701"
        });

        Assert.NotNull(result);
        Assert.Equal("New Address", result.AddressLine1);
        Assert.Equal("Durham", repository.Properties[0].City);
        Assert.Equal(7, repository.Properties[0].CustomerId);
    }

    private sealed class InMemoryPropertyRepository : IPropertyRepository
    {
        public List<Customer> Customers { get; } = new();

        public List<Property> Properties { get; } = new();

        public Task<IReadOnlyList<Property>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Property>>(Properties.ToList());
        }

        public Task<Property?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Properties.FirstOrDefault(property => property.Id == id));
        }

        public Task<IReadOnlyList<Property>> GetByCustomerIdAsync(
            int customerId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Property>>(
                Properties.Where(property => property.CustomerId == customerId).ToList());
        }

        public Task<bool> CustomerExistsAsync(
            int customerId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Customers.Any(customer => customer.Id == customerId));
        }

        public Task AddAsync(
            Property property,
            CancellationToken cancellationToken = default)
        {
            property.Id = Properties.Count == 0 ? 1 : Properties.Max(existing => existing.Id) + 1;
            Properties.Add(property);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}