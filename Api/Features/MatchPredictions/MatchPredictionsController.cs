using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.MatchPredictions;

[ApiController]
[Route("api/[controller]")]
public class MatchPredictionsController(IMatchPredictionService _matchPredictionService) : CustomBaseController
{
  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetAllAsync(
      orderBy: query => query.OrderByDescending(p => p.CreatedDate),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [Authorize]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetByIdAsync(
      id: id,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("match/{matchId:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetByMatchId(Guid matchId, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetByMatchIdAsync(
      matchId: matchId,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("match/{matchId:guid}/revealed")]
  [Authorize]
  public async Task<IActionResult> GetRevealedByMatchId(Guid matchId, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetRevealedByMatchIdAsync(
      matchId: matchId,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("match/{matchId:guid}/mine")]
  [Authorize]
  public async Task<IActionResult> GetMineByMatchId(Guid matchId, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetMineByMatchIdAsync(
      matchId: matchId,
      currentUserId: GetUserId(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}/mine")]
  [Authorize]
  public async Task<IActionResult> GetMineByCompetitionSeasonId(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetMineByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      currentUserId: GetUserId(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}/mine/points")]
  [Authorize]
  public async Task<IActionResult> GetMySeasonPoints(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetMySeasonPointsAsync(
      competitionSeasonId: competitionSeasonId,
      currentUserId: GetUserId(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("user/{userId:guid}")]
  [Authorize]
  public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
  {
    var result = await _matchPredictionService.GetByUserIdAsync(
      userId: userId,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize]
  public async Task<IActionResult> Add([FromBody] CreateMatchPredictionRequest request, CancellationToken cancellationToken)
  {
    var result = await _matchPredictionService.AddAsync(
      request: request,
      currentUserId: GetUserId(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize]
  public async Task<IActionResult> Update([FromBody] UpdateMatchPredictionRequest request, CancellationToken cancellationToken)
  {
    var result = await _matchPredictionService.UpdateAsync(
      request: request,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _matchPredictionService.RemoveAsync(
      id: id,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
