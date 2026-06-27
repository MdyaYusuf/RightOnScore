using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.CompetitionGroups;

[ApiController]
[Route("api/[controller]")]
public class CompetitionGroupsController(ICompetitionGroupService _competitionGroupService) : CustomBaseController
{
  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _competitionGroupService.GetAllAsync(
      orderBy: query => query.OrderBy(cg => cg.DisplayOrder),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("stage/{competitionStageId:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetByCompetitionStageId(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionGroupService.GetByCompetitionStageIdAsync(
      competitionStageId: competitionStageId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("stage/{competitionStageId:guid}/active")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActiveByCompetitionStageId(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    var result = await _competitionGroupService.GetActiveByCompetitionStageIdAsync(
      competitionStageId: competitionStageId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _competitionGroupService.GetByIdAsync(
      id: id,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Add([FromBody] CreateCompetitionGroupRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionGroupService.AddAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Update([FromBody] UpdateCompetitionGroupRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionGroupService.UpdateAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPatch("status")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangeStatus([FromBody] ChangeCompetitionGroupStatusRequest request, CancellationToken cancellationToken)
  {
    var result = await _competitionGroupService.ChangeStatusAsync(
      request: request,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _competitionGroupService.RemoveAsync(
      id: id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}
