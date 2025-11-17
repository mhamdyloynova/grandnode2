using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Grand.Module.MobileApi.Infrastructure;

public class MobileApiConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether Mobile API is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the secret key for JWT token generation
    /// </summary>
    public string SecretKey { get; set; } = "MobileApi-SecretKey-For-JWT-Token-Generation";

    /// <summary>
    /// Gets or sets the valid issuer for JWT tokens
    /// </summary>
    public string ValidIssuer { get; set; } = "GrandNode-MobileApi";

    /// <summary>
    /// Gets or sets the valid audience for JWT tokens
    /// </summary>
    public string ValidAudience { get; set; } = "GrandNode-MobileApi";

    /// <summary>
    /// Gets or sets a value indicating whether to validate the issuer
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to validate the audience
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Gets or sets the access token expiration time in minutes
    /// </summary>
    public int AccessTokenExpiration { get; set; } = 60; // 1 hour

    /// <summary>
    /// Gets or sets the refresh token expiration time in minutes
    /// </summary>
    public int RefreshTokenExpiration { get; set; } = 10080; // 7 days

    /// <summary>
    /// Gets or sets the authentication scheme name
    /// </summary>
    public const string AuthenticationScheme = "MobileApi";
}