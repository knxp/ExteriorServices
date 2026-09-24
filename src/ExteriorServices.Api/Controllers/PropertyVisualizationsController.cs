using ExteriorServices.Api.Models;
using ExteriorServices.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api/property-visualizations")]
public sealed class PropertyVisualizationsController : ControllerBase
{
    private readonly IPropertyVisualizationIntakeService _intakeService;
    private readonly IStoredVisualizationImageService _imageService;
    private readonly IPropertyVisualizationRevisionService _revisionService;

    public PropertyVisualizationsController(
        IPropertyVisualizationIntakeService intakeService,
        IStoredVisualizationImageService imageService,
        IPropertyVisualizationRevisionService revisionService)
    {
        _intakeService = intakeService;
        _imageService = imageService;
        _revisionService = revisionService;
    }

    [HttpPost("intake")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PropertyVisualizationIntakeResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<PropertyVisualizationIntakeResponse>> Intake(
        [FromForm] PropertyVisualizationIntakeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _intakeService.IntakeAsync(request, cancellationToken);
        return Accepted(response);
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<StoredVisualizationImage>> List()
    {
        return Ok(_imageService.List());
    }

    [HttpGet("{intakeId:guid}/source")]
    public IActionResult Source(Guid intakeId)
    {
        var image = _imageService.OpenSource(intakeId);
        return image is null ? NotFound() : File(image.Value.Stream, image.Value.ContentType);
    }

    [HttpGet("{intakeId:guid}/result")]
    public IActionResult Result(Guid intakeId)
    {
        var image = _imageService.OpenResult(intakeId);
        return image is null ? NotFound() : File(image.Value.Stream, image.Value.ContentType);
    }

    [HttpGet("{intakeId:guid}/revision")]
    public IActionResult Revision(Guid intakeId)
    {
        var image = _imageService.OpenRevision(intakeId);
        return image is null ? NotFound() : File(image.Value.Stream, image.Value.ContentType);
    }

    [HttpPost("{intakeId:guid}/revise")]
    public async Task<ActionResult<StoredVisualizationImage>> Revise(
        Guid intakeId,
        [FromBody] VisualizationReviseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _revisionService.ReviseAsync(intakeId, request.Notes, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{intakeId:guid}/approve")]
    public async Task<ActionResult<StoredVisualizationImage>> Approve(
        Guid intakeId,
        CancellationToken cancellationToken)
    {
        var result = await _revisionService.ApproveAsync(intakeId, cancellationToken);
        return Ok(result);
    }
}
