using Api.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.SeasonStandings;

[ApiController]
[Route("api/[controller]")]
public class SeasonStandingsController(ISeasonStandingService _seasonStandingService) : CustomBaseController
{
  [HttpGet("season/{competitionSeasonId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetTopByCompetitionSeasonId(
    Guid competitionSeasonId,
    [FromQuery] int topCount = 50,
    CancellationToken cancellationToken = default)
  {
    var result = await _seasonStandingService.GetTopByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      topCount: topCount,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("season/{competitionSeasonId:guid}/mine")]
  [Authorize]
  public async Task<IActionResult> GetMineByCompetitionSeasonId(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    var result = await _seasonStandingService.GetMineByCompetitionSeasonIdAsync(
      competitionSeasonId: competitionSeasonId,
      currentUserId: GetUserId(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
