using Grand.SharedKernel.Extensions;

namespace Grand.Module.MobileApi.Constants;

public static class MobileApiConstants
{
    /// <summary>
    /// Mobile API Group Name (v3)
    /// </summary>
    public const string ApiGroupName = ApiConstants.ApiGroupNameV3;

    /// <summary>
    /// Mobile API base route
    /// </summary>
    public const string BaseRoute = "mobile-api";

    /// <summary>
    /// Default mobile API response version
    /// </summary>
    public const string ApiVersion = "v3";

    /// <summary>
    /// User types for JWT claims
    /// </summary>
    public static class UserTypes
    {
        public const string Guest = "guest";
        public const string Registered = "registered";
    }

    /// <summary>
    /// Error codes for mobile API responses
    /// </summary>
    public static class ErrorCodes
    {
        public const string ValidationError = "VALIDATION_ERROR";
        public const string AuthenticationError = "AUTHENTICATION_ERROR";
        public const string AuthorizationError = "AUTHORIZATION_ERROR";
        public const string NotFoundError = "NOT_FOUND_ERROR";
        public const string ConflictError = "CONFLICT_ERROR";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string BadRequestError = "BAD_REQUEST_ERROR";
    }
}