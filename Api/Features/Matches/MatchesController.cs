using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Matches;

[ApiController]
[Route("api/[controller]")]
public class MatchesController(IMatchService _matchService) : CustomBaseController
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _matchService.GetAllAsync(
      orderBy: query => query.OrderBy(m => m.KickoffTime),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _matchService.GetByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}/upcoming")]
  [AllowAnonymous]
  public async Task<IActionResult> GetUpcomingByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _matchService.GetUpcomingByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("stage/{competitionStageId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionStageId(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    var result = await _matchService.GetByCompetitionStageIdAsync(
      competitionStageId: competitionStageId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("group/{competitionGroupId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionGroupId(Guid competitionGroupId, CancellationToken cancellationToken = default)
  {
    var result = await _matchService.GetByCompetitionGroupIdAsync(
      competitionGroupId: competitionGroupId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _matchService.GetByIdAsync(
      id: id,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Add([FromBody] CreateMatchRequest request, CancellationToken cancellationToken)
  {
    var result = await _matchService.AddAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Update([FromBody] UpdateMatchRequest request, CancellationToken cancellationToken)
  {
    var result = await _matchService.UpdateAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangeStatus([FromBody] ChangeMatchStatusRequest request, CancellationToken cancellationToken)
  {
    var result = await _matchService.ChangeStatusAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("result")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> RecordResult([FromBody] RecordMatchResultRequest request, CancellationToken cancellationToken)
  {
    var result = await _matchService.RecordResultAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _matchService.RemoveAsync(
      id: id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
