using ExteriorServices.Application.Jobs;
using ExteriorServices.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<IReadOnlyList<JobDto>>> GetJobs(
        CancellationToken cancellationToken)
    {
        return Ok(await _jobService.GetJobsAsync(cancellationToken));
    }

    [HttpGet("jobs/{id:int}")]
    public async Task<ActionResult<JobDto>> GetJob(
        int id,
        CancellationToken cancellationToken)
    {
        var job = await _jobService.GetJobAsync(id, cancellationToken);

        if (job is null)
        {
            return NotFound(new ApiError("JobNotFound", "The requested job could not be found."));
        }

        return Ok(job);
    }

    [HttpGet("customers/{customerId:int}/jobs")]
    public async Task<ActionResult<IReadOnlyList<JobDto>>> GetCustomerJobs(
        int customerId,
        CancellationToken cancellationToken)
    {
        return Ok(await _jobService.GetCustomerJobsAsync(customerId, cancellationToken));
    }

    [HttpPost("customers/{customerId:int}/jobs")]
    public async Task<ActionResult<JobDto>> CreateJobForCustomer(
        int customerId,
        [FromBody] CreateJobRequest request,
        CancellationToken cancellationToken)
    {
        var job = await _jobService.CreateJobAsync(customerId, request, cancellationToken);

        if (job is null)
        {
            return NotFound(new ApiError("CustomerNotFound", "The requested customer could not be found."));
        }

        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    [HttpPut("jobs/{id:int}")]
    public async Task<ActionResult<JobDto>> UpdateJob(
        int id,
        [FromBody] UpdateJobRequest request,
        CancellationToken cancellationToken)
    {
        var job = await _jobService.UpdateJobAsync(id, request, cancellationToken);

        if (job is null)
        {
            return NotFound(new ApiError("JobNotFound", "The requested job could not be found."));
        }

        return Ok(job);
    }
}
