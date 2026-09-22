using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public interface IJobApiClient
{
    Task<IReadOnlyList<JobSummary>> GetJobsAsync(CancellationToken cancellationToken = default);

    Task<JobSummary?> CreateJobAsync(int customerId, JobCreateRequest request, CancellationToken cancellationToken = default);
}
