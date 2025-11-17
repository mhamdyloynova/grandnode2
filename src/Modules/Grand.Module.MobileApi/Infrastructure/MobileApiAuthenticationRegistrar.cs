using Grand.Business.Core.Interfaces.Authentication;
using Grand.Infrastructure.Extensions;
using Grand.Module.MobileApi.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Grand.Module.MobileApi.Infrastructure;

public class MobileApiAuthenticationRegistrar : IAuthenticationBuilder
{
    public void AddAuthentication(AuthenticationBuilder builder, IConfiguration configuration)
    {
        // Register Mobile API configuration
        var mobileApiConfig = new MobileApiConfig();
        configuration.GetSection("MobileAPI").Bind(mobileApiConfig);

        if (!mobileApiConfig.Enabled)
            return;

        // Configure JWT Bearer authentication for Mobile API
        builder.AddJwtBearer(MobileApiConfig.AuthenticationScheme, options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(mobileApiConfig.SecretKey);
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = mobileApiConfig.ValidateIssuer,
                ValidIssuer = mobileApiConfig.ValidateIssuer ? mobileApiConfig.ValidIssuer : null,
                ValidateAudience = mobileApiConfig.ValidateAudience,
                ValidAudience = mobileApiConfig.ValidateAudience ? mobileApiConfig.ValidAudience : null,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    // Allow token from Authorization header only
                    var authorization = context.Request.Headers.Authorization.FirstOrDefault();
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                    {
                        context.Token = authorization.Substring("Bearer ".Length);
                    }
                    return Task.CompletedTask;
                }
            };
        });
    }

    public int Priority => 910; // After ApiAuthenticationRegistrar (900) but before plugins
}