using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.CompetitionTeams;

[ApiController]
[Route("api/[controller]")]
public class CompetitionTeamsController(ICompetitionTeamService _competitionTeamService) : CustomBaseController
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _competitionTeamService.GetAllAsync(
      orderBy: query => query.OrderBy(ct => ct.Seed ?? int.MaxValue).ThenBy(ct => ct.TeamId),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionTeamService.GetByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}/active")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActiveByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionTeamService.GetActiveByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("stage/{competitionStageId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionStageId(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionTeamService.GetByCompetitionStageIdAsync(
      competitionStageId: competitionStageId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("group/{competitionGroupId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionGroupId(Guid competitionGroupId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionTeamService.GetByCompetitionGroupIdAsync(
      competitionGroupId: competitionGroupId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _competitionTeamService.GetByIdAsync(
      id: id,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Add([FromBody] CreateCompetitionTeamRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionTeamService.AddAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Update([FromBody] UpdateCompetitionTeamRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionTeamService.UpdateAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangeStatus([FromBody] ChangeCompetitionTeamStatusRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionTeamService.ChangeStatusAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _competitionTeamService.RemoveAsync(
      id: id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
