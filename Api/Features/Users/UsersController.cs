using Api.Core.Controllers;
using Api.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Users;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService _userService) : CustomBaseController
{
  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken = default)
  {
    var result = await _userService.GetAllAsync(
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      pageNumber: pagination.PageNumber,
      pageSize: pagination.PageSize,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
  {
    var result = await _userService.GetByIdAsync(
      id: id,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("by-email/{email}")]
  public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
  {
    var result = await _userService.GetAsync(
      predicate: u => u.Email == email,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [Authorize(Roles = "Admin")]
  [HttpGet("newest-members/{count:int}")]
  public async Task<IActionResult> GetNewestMembers(
    int count,
    [FromQuery] DateTime? lastDate = null,
    [FromQuery] Guid? lastId = null,
    CancellationToken cancellationToken = default)
  {
    var result = await _userService.GetNewestMembersAsync(
      count: count,
      lastDateCursor: lastDate,
      lastIdCursor: lastId,
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [AllowAnonymous]
  [HttpGet("check-email")]
  public async Task<IActionResult> CheckEmailUnique([FromQuery] string email, CancellationToken cancellationToken)
  {
    var result = await _userService.CheckEmailUniqueAsync(email, cancellationToken);
    return CreateActionResult(result);
  }

  [HttpPut("profile")]
  public async Task<IActionResult> Update([FromForm] UpdateUserRequest request, CancellationToken cancellationToken)
  {
    var result = await _userService.UpdateAsync(
      request: request,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPut("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
  {
    var result = await _userService.ChangePasswordAsync(
      request: request,
      currentUserId: GetUserId(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _userService.RemoveAsync(
      id: id,
      currentUserId: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }

  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
  {
    Guid userId = GetUserId();

    var result = await _userService.GetByIdAsync(
      id: userId,
      currentUserId: userId,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return CreateActionResult(result);
  }
}