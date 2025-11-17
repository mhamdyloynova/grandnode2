namespace Grand.Module.MobileApi.DTOs;

/// <summary>
/// Mobile authentication token response
/// </summary>
public class MobileAuthTokenDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for renewing access token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// User type (guest, registered)
    /// </summary>
    public string UserType { get; set; } = string.Empty;

    /// <summary>
    /// Customer GUID
    /// </summary>
    public string CustomerGuid { get; set; } = string.Empty;

    /// <summary>
    /// Customer information (for registered users)
    /// </summary>
    public MobileCustomerInfoDto? CustomerInfo { get; set; }
}

/// <summary>
/// Mobile customer information
/// </summary>
public class MobileCustomerInfoDto
{
    /// <summary>
    /// Customer email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}

/// <summary>
/// Mobile login request
/// </summary>
public class MobileLoginDto
{
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Mobile registration request
/// </summary>
public class MobileRegisterDto
{
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirm password
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Mobile token refresh request
/// </summary>
public class MobileRefreshTokenDto
{
    /// <summary>
    /// Access token to refresh
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}