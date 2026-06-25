using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.CompetitionStages;

[ApiController]
[Route("api/[controller]")]
public class CompetitionStagesController(ICompetitionStageService _competitionStageService) : CustomBaseController
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _competitionStageService.GetAllAsync(
      orderBy: query => query.OrderBy(cs => cs.DisplayOrder),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionStageService.GetByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}/active")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActiveByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionStageService.GetActiveByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _competitionStageService.GetByIdAsync(
      id: id,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Add([FromBody] CreateCompetitionStageRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionStageService.AddAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Update([FromBody] UpdateCompetitionStageRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionStageService.UpdateAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangeStatus([FromBody] ChangeCompetitionStageStatusRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionStageService.ChangeStatusAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _competitionStageService.RemoveAsync(
      id: id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
