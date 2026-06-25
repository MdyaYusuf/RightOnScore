using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.CompetitionSeasons;

[ApiController]
[Route("api/[controller]")]
public class CompetitionSeasonsController(ICompetitionSeasonService _competitionSeasonService) : CustomBaseController
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _competitionSeasonService.GetAllAsync(
      orderBy: query => query.OrderByDescending(cs => cs.StartDate),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("active")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
  {
    var result = await _competitionSeasonService.GetActiveAsync(cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("competition/{competitionId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionId(Guid competitionId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionSeasonService.GetByCompetitionIdAsync(
      competitionId: competitionId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("competition/{competitionId:guid}/active")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActiveByCompetitionId(Guid competitionId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionSeasonService.GetActiveByCompetitionIdAsync(
      competitionId: competitionId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _competitionSeasonService.GetByIdAsync(
      id: id,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Add([FromBody] CreateCompetitionSeasonRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionSeasonService.AddAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Update([FromBody] UpdateCompetitionSeasonRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionSeasonService.UpdateAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangeStatus([FromBody] ChangeCompetitionSeasonStatusRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionSeasonService.ChangeStatusAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _competitionSeasonService.RemoveAsync(
      id: id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
