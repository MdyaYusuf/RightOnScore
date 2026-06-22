using System.Text.Json;
using Api.Core.Exceptions;
using Api.Core.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Core.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    var statusCode = exception switch
    {
      NotFoundException => StatusCodes.Status404NotFound,
      AuthorizationException => StatusCodes.Status401Unauthorized,
      ForbiddenException => StatusCodes.Status403Forbidden,
      BusinessException => StatusCodes.Status400BadRequest,
      ValidationException => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    var response = new ReturnModel<NoData>
    {
      Success = false,
      Message = statusCode == 500 ? "Sistem kaynaklı bir hata oluştu." : exception.Message,
      StatusCode = statusCode
    };

    if (exception is ValidationException validationException)
    {
      response.Message = "Validasyon hataları oluştu.";
      response.Errors = validationException.Errors.Select(x => x.ErrorMessage).ToList();
    }

    httpContext.Response.ContentType = "application/json";
    httpContext.Response.StatusCode = statusCode;

    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions), cancellationToken);

    return true;
  }
}