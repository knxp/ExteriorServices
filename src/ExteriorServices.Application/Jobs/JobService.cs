using ExteriorServices.Domain.Entities;

namespace ExteriorServices.Application.Jobs;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<IReadOnlyList<JobDto>> GetJobsAsync(
        CancellationToken cancellationToken = default)
    {
        var jobs = await _jobRepository.GetAllAsync(cancellationToken);
        return jobs.Select(Map).ToList();
    }

    public async Task<JobDto?> GetJobAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);
        return job is null ? null : Map(job);
    }

    public async Task<IReadOnlyList<JobDto>> GetCustomerJobsAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var jobs = await _jobRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return jobs.Select(Map).ToList();
    }

    public async Task<JobDto?> CreateJobAsync(
        int customerId,
        CreateJobRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _jobRepository.CustomerExistsAsync(customerId, cancellationToken))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var job = new Job
        {
            CustomerId = customerId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Phone = request.Phone,
            AddressLine1 = request.AddressLine1.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Estimate = request.Estimate,
            UpFront = request.UpFront,
            JobTotal = request.JobTotal,
            ContactDate = request.ContactDate,
            InstallDate = request.InstallDate,
            TeardownDate = request.TeardownDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _jobRepository.AddAsync(job, cancellationToken);
        await _jobRepository.SaveChangesAsync(cancellationToken);

        return Map(job);
    }

    public async Task<JobDto?> UpdateJobAsync(
        int id,
        UpdateJobRequest request,
        CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);
        if (job is null)
        {
            return null;
        }

        job.FirstName = request.FirstName.Trim();
        job.LastName = request.LastName.Trim();
        job.Phone = request.Phone;
        job.AddressLine1 = request.AddressLine1.Trim();
        job.City = request.City.Trim();
        job.PostalCode = request.PostalCode.Trim();
        job.Estimate = request.Estimate;
        job.UpFront = request.UpFront;
        job.JobTotal = request.JobTotal;
        job.ContactDate = request.ContactDate;
        job.InstallDate = request.InstallDate;
        job.TeardownDate = request.TeardownDate;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.SaveChangesAsync(cancellationToken);
        return Map(job);
    }

    private static JobDto Map(Job job) => new()
    {
        Id = job.Id,
        CustomerId = job.CustomerId,
        FirstName = job.FirstName,
        LastName = job.LastName,
        Phone = job.Phone,
        AddressLine1 = job.AddressLine1,
        City = job.City,
        PostalCode = job.PostalCode,
        Estimate = job.Estimate,
        UpFront = job.UpFront,
        JobTotal = job.JobTotal,
        ContactDate = job.ContactDate,
        InstallDate = job.InstallDate,
        TeardownDate = job.TeardownDate,
        CreatedAt = job.CreatedAt,
        UpdatedAt = job.UpdatedAt
    };
}
