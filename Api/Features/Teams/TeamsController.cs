using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Teams;

[ApiController]
[Route("api/[controller]")]
public class TeamsController(ITeamService _teamService) : CustomBaseController
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _teamService.GetAllAsync(
      orderBy: query => query.OrderBy(t => t.Name),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("active")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
  {
    var result = await _teamService.GetActiveAsync(cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("country/{country}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCountry(string country, CancellationToken cancellationToken = default)
  {
    var result = await _teamService.GetByCountryAsync(
      country: country,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("search")]
  [AllowAnonymous]
  public async Task<IActionResult> Search([FromQuery] SearchTeamRequest request, CancellationToken cancellationToken = default)
  {
    var result = await _teamService.SearchAsync(
      request: request,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _teamService.GetByIdAsync(
      id: id,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Add([FromForm] CreateTeamRequest request, CancellationToken cancellationToken)
  {
    var result = await _teamService.AddAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Update([FromForm] UpdateTeamRequest request, CancellationToken cancellationToken)
  {
    var result = await _teamService.UpdateAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangeStatus([FromBody] ChangeTeamStatusRequest request, CancellationToken cancellationToken)
  {
    var result = await _teamService.ChangeStatusAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _teamService.RemoveAsync(
      id: id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
