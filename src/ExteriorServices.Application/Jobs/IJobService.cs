namespace ExteriorServices.Application.Jobs;

public interface IJobService
{
    Task<IReadOnlyList<JobDto>> GetJobsAsync(
        CancellationToken cancellationToken = default);

    Task<JobDto?> GetJobAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobDto>> GetCustomerJobsAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<JobDto?> CreateJobAsync(
        int customerId,
        CreateJobRequest request,
        CancellationToken cancellationToken = default);

    Task<JobDto?> UpdateJobAsync(
        int id,
        UpdateJobRequest request,
        CancellationToken cancellationToken = default);
}
