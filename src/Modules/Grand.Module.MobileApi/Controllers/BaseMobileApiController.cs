using Grand.Module.MobileApi.Constants;
using Grand.Module.MobileApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Module.MobileApi.Controllers;

/// <summary>
/// Base controller for Mobile API endpoints
/// </summary>
[ApiExplorerSettings(GroupName = MobileApiConstants.ApiGroupName)]
[ApiController]
[Route($"{MobileApiConstants.BaseRoute}/[controller]")]
[Produces("application/json")]
public abstract class BaseMobileApiController : ControllerBase
{
    /// <summary>
    /// Return success response with data
    /// </summary>
    protected IActionResult Success<T>(T data, string? message = null)
    {
        return Ok(MobileResponseDto<T>.CreateSuccess(data, message));
    }

    /// <summary>
    /// Return error response with details
    /// </summary>
    protected IActionResult Error(string code, string message, object? details = null, int statusCode = 400)
    {
        var response = MobileResponseDto<object>.CreateError(code, message, details);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Return validation error response
    /// </summary>
    protected IActionResult ValidationError(string message, object? details = null)
    {
        return Error(MobileApiConstants.ErrorCodes.ValidationError, message, details, 422);
    }

    /// <summary>
    /// Return authentication error response
    /// </summary>
    protected IActionResult AuthenticationError(string message = "Authentication required")
    {
        return Error(MobileApiConstants.ErrorCodes.AuthenticationError, message, null, 401);
    }

    /// <summary>
    /// Return authorization error response
    /// </summary>
    protected IActionResult AuthorizationError(string message = "Access denied")
    {
        return Error(MobileApiConstants.ErrorCodes.AuthorizationError, message, null, 403);
    }

    /// <summary>
    /// Return not found error response
    /// </summary>
    protected IActionResult NotFoundError(string message = "Resource not found")
    {
        return Error(MobileApiConstants.ErrorCodes.NotFoundError, message, null, 404);
    }

    /// <summary>
    /// Return conflict error response
    /// </summary>
    protected IActionResult ConflictError(string message = "Conflict occurred")
    {
        return Error(MobileApiConstants.ErrorCodes.ConflictError, message, null, 409);
    }

    /// <summary>
    /// Return internal server error response
    /// </summary>
    protected IActionResult InternalServerError(string message = "Internal server error occurred")
    {
        return Error(MobileApiConstants.ErrorCodes.InternalServerError, message, null, 500);
    }
}