using Grand.Business.Core.Interfaces.Authentication;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Utilities.Customers;
using Grand.Business.Core.Events.Customers;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Security;
using Grand.Infrastructure;
using Grand.Module.MobileApi.Constants;
using Grand.Module.MobileApi.DTOs;
using Grand.Module.MobileApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;

namespace Grand.Module.MobileApi.Controllers;

/// <summary>
/// Mobile API Authentication Controller
/// Handles guest sessions, user login/registration, and token management
/// </summary>
[ApiExplorerSettings(GroupName = "v3")]
public class AuthController : BaseMobileApiController
{
    private readonly ICustomerService _customerService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IContextAccessor _contextAccessor;
    private readonly MobileApiConfig _config;
    private readonly ICustomerManagerService _customerManagerService;
    private readonly CustomerSettings _customerSettings;
    private readonly IMediator _mediator;

    public AuthController(
        ICustomerService customerService,
        IRefreshTokenService refreshTokenService,
        IContextAccessor contextAccessor,
        MobileApiConfig config,
        ICustomerManagerService customerManagerService,
        CustomerSettings customerSettings,
        IMediator mediator)
    {
        _customerService = customerService;
        _refreshTokenService = refreshTokenService;
        _contextAccessor = contextAccessor;
        _config = config;
        _customerManagerService = customerManagerService;
        _customerSettings = customerSettings;
        _mediator = mediator;
    }

    /// <summary>
    /// Create guest user session
    /// </summary>
    /// <returns>Guest user token for anonymous shopping</returns>
    [HttpPost("guest")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateGuest()
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        try
        {
            // Create guest customer
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Active = true,
                StoreId = _contextAccessor.StoreContext.CurrentStore.Id,
                LastActivityDateUtc = DateTime.UtcNow
            };

            customer = await _customerService.InsertGuestCustomer(customer);

            // Generate tokens
            var tokenResponse = await GenerateTokenResponse(customer, MobileApiConstants.UserTypes.Guest);

            return Success(tokenResponse, "Guest session created successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Failed to create guest session: {ex.Message}");
        }
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="model">Login credentials</param>
    /// <returns>Authentication token for registered user</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] MobileLoginDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid login data", ModelState);

        try
        {
            // Validate customer credentials using CustomerManagerService (same as frontend)
            var loginResult = await _customerManagerService.LoginCustomer(model.Email, model.Password);

            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    // Get customer for token generation
                    var customer = await _customerService.GetCustomerByEmail(model.Email);
                    if (customer == null)
                        return AuthenticationError("Invalid email or password");

                    // Generate tokens for registered user
                    var tokenResponse = await GenerateTokenResponse(customer, MobileApiConstants.UserTypes.Registered);
                    return Success(tokenResponse, "Login successful");

                case CustomerLoginResults.CustomerNotExist:
                    return AuthenticationError("Invalid email or password");

                case CustomerLoginResults.Deleted:
                    return AuthenticationError("Account has been deleted");

                case CustomerLoginResults.NotActive:
                    return AuthenticationError("Account is not active");

                case CustomerLoginResults.NotRegistered:
                    return AuthenticationError("Account is not registered");

                case CustomerLoginResults.LockedOut:
                    return AuthenticationError("Account is locked out");

                case CustomerLoginResults.WrongPassword:
                    return AuthenticationError("Invalid email or password");

                case CustomerLoginResults.RequiresTwoFactor:
                    return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Two-factor authentication required");

                default:
                    return AuthenticationError("Login failed");
            }
        }
        catch (Exception ex)
        {
            return InternalServerError($"Login failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Register new user or convert guest to registered user
    /// </summary>
    /// <param name="model">Registration data</param>
    /// <returns>Authentication token for new registered user</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] MobileRegisterDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid registration data", ModelState);

        if (model.Password != model.ConfirmPassword)
            return ValidationError("Passwords do not match");

        try
        {
            // Check if email already exists
            var existingCustomer = await _customerService.GetCustomerByEmail(model.Email);
            if (existingCustomer != null && !string.IsNullOrEmpty(existingCustomer.Email))
                return ConflictError("Email address is already registered");

            Customer customer;

            // Check if user is converting from guest (has authorization header)
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                // Convert guest to registered user
                var token = authHeader.Substring("Bearer ".Length);
                var principal = ValidateToken(token);
                
                if (principal != null)
                {
                    var guid = principal.Claims.FirstOrDefault(x => x.Type == "Guid")?.Value;
                    if (!string.IsNullOrEmpty(guid))
                    {
                        customer = await _customerService.GetCustomerByGuid(Guid.Parse(guid));
                        if (customer == null)
                        {
                            // Create new customer if guest not found
                            customer = new Customer
                            {
                                CustomerGuid = Guid.NewGuid(),
                                Active = true,
                                StoreId = _contextAccessor.StoreContext.CurrentStore.Id,
                                LastActivityDateUtc = DateTime.UtcNow
                            };
                            await _customerService.InsertCustomer(customer);
                        }
                    }
                    else
                    {
                        // Create new customer
                        customer = new Customer
                        {
                            CustomerGuid = Guid.NewGuid(),
                            Active = true,
                            StoreId = _contextAccessor.StoreContext.CurrentStore.Id,
                            LastActivityDateUtc = DateTime.UtcNow
                        };
                        await _customerService.InsertCustomer(customer);
                    }
                }
                else
                {
                    // Create new customer
                    customer = new Customer
                    {
                        CustomerGuid = Guid.NewGuid(),
                        Active = true,
                        StoreId = _contextAccessor.StoreContext.CurrentStore.Id,
                        LastActivityDateUtc = DateTime.UtcNow
                    };
                    await _customerService.InsertCustomer(customer);
                }
            }
            else
            {
                // Create new customer
                customer = new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    Active = true,
                    StoreId = _contextAccessor.StoreContext.CurrentStore.Id,
                    LastActivityDateUtc = DateTime.UtcNow
                };
                await _customerService.InsertCustomer(customer);
            }

            // Register the customer using CustomerManagerService (handles password hashing and role assignment)
            var registrationRequest = new RegistrationRequest(
                customer,
                model.Email,
                model.Email, // Use email as username
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                _contextAccessor.StoreContext.CurrentStore.Id,
                isApproved: true // Mobile registrations are auto-approved
            );
            
            await _customerManagerService.RegisterCustomer(registrationRequest);

            // Store FirstName and LastName (following frontend registration pattern)
            await _customerService.UpdateUserField(customer, SystemCustomerFieldNames.FirstName, model.FirstName);
            await _customerService.UpdateUserField(customer, SystemCustomerFieldNames.LastName, model.LastName);

            // Publish CustomerRegisteredEvent (for loyalty points and other event handlers)
            await _mediator.Publish(new CustomerRegisteredEvent(customer));

            // Generate tokens for registered user
            var tokenResponse = await GenerateTokenResponse(customer, MobileApiConstants.UserTypes.Registered);

            return Success(tokenResponse, "Registration successful");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Registration failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="model">Refresh token data</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] MobileRefreshTokenDto model)
    {
        if (!_config.Enabled)
            return Error(MobileApiConstants.ErrorCodes.BadRequestError, "Mobile API is disabled");

        if (!ModelState.IsValid)
            return ValidationError("Invalid refresh token data", ModelState);

        try
        {
            var principal = ValidateToken(model.AccessToken);
            if (principal == null)
                return AuthenticationError("Invalid access token");

            // Get customer from token
            Customer customer;
            var email = principal.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                customer = await _customerService.GetCustomerByEmail(email);
            }
            else
            {
                var guid = principal.Claims.FirstOrDefault(x => x.Type == "Guid")?.Value;
                if (!string.IsNullOrEmpty(guid))
                {
                    customer = await _customerService.GetCustomerByGuid(Guid.Parse(guid));
                }
                else
                {
                    return AuthenticationError("Invalid token claims");
                }
            }

            if (customer == null)
                return AuthenticationError("Customer not found");

            // Validate refresh token
            var customerRefreshToken = await _refreshTokenService.GetCustomerRefreshToken(customer);
            if (customerRefreshToken == null || customerRefreshToken.Token != model.RefreshToken)
                return AuthenticationError("Invalid refresh token");

            if (customerRefreshToken.ValidTo < DateTime.UtcNow)
                return AuthenticationError("Refresh token expired");

            // Generate new tokens
            var userType = !string.IsNullOrEmpty(customer.Email) ? 
                MobileApiConstants.UserTypes.Registered : 
                MobileApiConstants.UserTypes.Guest;

            var tokenResponse = await GenerateTokenResponse(customer, userType);

            return Success(tokenResponse, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Token refresh failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Logout user and invalidate tokens
    /// </summary>
    /// <returns>Logout confirmation</returns>
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = MobileApiConfig.AuthenticationScheme)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Get customer from current context
            var customer = await GetCurrentCustomer();
            if (customer != null)
            {
                // Invalidate refresh token by removing it from customer
                await _customerService.UpdateUserField(customer, SystemCustomerFieldNames.RefreshToken, (RefreshToken)null);
            }

            return Success<object>(null, "Logout successful");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Logout failed: {ex.Message}");
        }
    }

    #region Private Methods

    private async Task<MobileAuthTokenDto> GenerateTokenResponse(Customer customer, string userType)
    {
        // Create claims
        var claims = new List<Claim>
        {
            new Claim("Guid", customer.CustomerGuid.ToString()),
            new Claim("UserType", userType),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrEmpty(customer.Email))
        {
            claims.Add(new Claim("Email", customer.Email));
        }

        // Generate refresh token
        var refreshTokenValue = _refreshTokenService.GenerateRefreshToken();
        var refreshToken = await _refreshTokenService.SaveRefreshTokenToCustomer(customer, refreshTokenValue);
        claims.Add(new Claim("RefreshId", refreshToken.RefreshId));

        // Generate access token
        var accessToken = GenerateAccessToken(claims);

        // Create response
        var response = new MobileAuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresIn = _config.AccessTokenExpiration * 60, // Convert minutes to seconds
            UserType = userType,
            CustomerGuid = customer.CustomerGuid.ToString()
        };

        // Add customer info for registered users
        if (userType == MobileApiConstants.UserTypes.Registered && !string.IsNullOrEmpty(customer.Email))
        {
            response.CustomerInfo = new MobileCustomerInfoDto
            {
                Email = customer.Email,
                FirstName = customer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.FirstName) ?? "",
                LastName = customer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.LastName) ?? ""
            };
            response.CustomerInfo.FullName = $"{response.CustomerInfo.FirstName} {response.CustomerInfo.LastName}".Trim();
        }

        return response;
    }

    private string GenerateAccessToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config.ValidateIssuer ? _config.ValidIssuer : null,
            audience: _config.ValidateAudience ? _config.ValidAudience : null,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.AccessTokenExpiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = _config.ValidateIssuer,
                ValidIssuer = _config.ValidateIssuer ? _config.ValidIssuer : null,
                ValidateAudience = _config.ValidateAudience,
                ValidAudience = _config.ValidateAudience ? _config.ValidAudience : null,
                ValidateLifetime = false, // Don't validate lifetime for refresh
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private async Task<Customer?> GetCurrentCustomer()
    {
        var email = User.FindFirst("Email")?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            return await _customerService.GetCustomerByEmail(email);
        }

        var guid = User.FindFirst("Guid")?.Value;
        if (!string.IsNullOrEmpty(guid) && Guid.TryParse(guid, out var customerGuid))
        {
            return await _customerService.GetCustomerByGuid(customerGuid);
        }

        return null;
    }

    #endregion
}