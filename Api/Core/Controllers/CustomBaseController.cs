using System.Security.Claims;
using Api.Core.Exceptions;
using Api.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Controllers;

public class CustomBaseController : ControllerBase
{
  [NonAction]
  public IActionResult CreateActionResult<T>(ReturnModel<T> result)
  {
    return new ObjectResult(result)
    {
      StatusCode = result.StatusCode
    };
  }

  [NonAction]
  protected Guid GetUserId()
  {
    string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid guid))
    {
      throw new AuthorizationException("İşlem için giriş yapmanız gerekmektedir.");
    }

    return guid;
  }

  [NonAction]
  protected Guid? TryGetUserId()
  {
    string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    return Guid.TryParse(userId, out var id) ? id : null;
  }

  [NonAction]
  protected string GetUserRole()
  {
    return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
  }
}