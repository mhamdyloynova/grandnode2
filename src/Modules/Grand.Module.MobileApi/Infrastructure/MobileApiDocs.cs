using Grand.Module.MobileApi.Constants;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Grand.Module.MobileApi.Infrastructure;

/// <summary>
/// Simple OpenAPI configuration for Mobile API
/// </summary>
public static class MobileApiDocs
{
    /// <summary>
    /// Get Mobile API OpenAPI info
    /// </summary>
    /// <returns>OpenAPI info</returns>
    public static OpenApiInfo GetApiInfo()
    {
        return new OpenApiInfo
        {
            Title = "GrandNode2 Mobile API",
            Version = "v3",
            Description = "Mobile-optimized REST API for GrandNode2 e-commerce platform",
            Contact = new OpenApiContact
            {
                Name = "GrandNode API Support",
                Email = "support@grandnode.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT License"
            }
        };
    }

    /// <summary>
    /// Get JWT security scheme
    /// </summary>
    /// <returns>Security scheme for JWT</returns>
    public static OpenApiSecurityScheme GetJwtSecurityScheme()
    {
        return new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
        };
    }

    /// <summary>
    /// Get JWT security requirement
    /// </summary>
    /// <returns>Security requirement for JWT</returns>
    public static OpenApiSecurityRequirement GetJwtSecurityRequirement()
    {
        return new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new[] {"Bearer"}
            }
        };
    }
}