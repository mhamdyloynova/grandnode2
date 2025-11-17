using Grand.Infrastructure;
using Grand.Infrastructure.Configuration;
using Grand.Module.Api.ApiExplorer;
using Grand.Module.Api.Attributes;
using Grand.Module.Api.Infrastructure.Extensions;
using Grand.Module.Api.Infrastructure.Transformers;
using Grand.SharedKernel.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Grand.Module.Api.Infrastructure;

public class OpenApiStartup : IStartupApplication
{
    public void Configure(WebApplication application, IWebHostEnvironment webHostEnvironment)
    {
        application.MapOpenApi();
        if (application.Environment.IsDevelopment())
        {
            application.MapScalarApiReference(options =>
            {
                options.WithTitle("GrandNode2 API Documentation");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.WithTheme(ScalarTheme.Purple);
            });
            
            // Map individual API versions for Scalar
            application.MapScalarApiReference("v1", options =>
            {
                options.WithTitle("Admin API v1 - GrandNode2");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.WithTheme(ScalarTheme.Default);
            });
            
            application.MapScalarApiReference("v2", options =>
            {
                options.WithTitle("Frontend API v2 - GrandNode2");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.WithTheme(ScalarTheme.Default);
            });
            
            application.MapScalarApiReference("v3", options =>
            {
                options.WithTitle("Mobile API v3 - GrandNode2");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.WithTheme(ScalarTheme.Purple);
            });
        }
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var backendApiConfig = services.BuildServiceProvider().GetService<BackendAPIConfig>();
        var frontApiConfig = services.BuildServiceProvider().GetService<FrontendAPIConfig>();
        var webHostEnvironment = services.BuildServiceProvider().GetService<IWebHostEnvironment>();
        
        // Check if Mobile API is enabled
        var mobileApiEnabled = configuration.GetSection("FeatureManagement:Grand.Module.MobileApi").Get<bool>();

        if (webHostEnvironment.IsDevelopment() && (backendApiConfig.Enabled || frontApiConfig.Enabled || mobileApiEnabled))
        {
            if (backendApiConfig.Enabled)
            {
                ConfigureBackendApi(services);
            }
            if (frontApiConfig.Enabled)
            {
                ConfigureFrontendApi(services);
            }
            if (mobileApiEnabled)
            {
                ConfigureMobileApi(services);
            }
        }

        if (backendApiConfig.Enabled)
        {
            //register RequestHandler
            services.RegisterRequestHandler();

            //Add JsonPatchInputFormatter
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, services.GetJsonPatchInputFormatter());
            });
            services.AddScoped<ModelValidationAttribute>();
        }
    }
    public int Priority => 505;
    public bool BeforeConfigure => false;

    private static void ConfigureBackendApi(IServiceCollection services)
    {
        services.AddOpenApi(ApiConstants.ApiGroupNameV1, options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
            options.AddContactDocumentTransformer("Grandnode Backend API", ApiConstants.ApiGroupNameV1);
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddSchemaTransformer<EnumSchemaTransformer>();
            options.AddOperationTransformer();
            options.AddClearServerDocumentTransformer();
        });
    }

    private static void ConfigureFrontendApi(IServiceCollection services)
    {
        services.AddOpenApi(ApiConstants.ApiGroupNameV2, options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
            options.AddContactDocumentTransformer("Grandnode Frontend API", ApiConstants.ApiGroupNameV2);
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddSchemaTransformer<EnumSchemaTransformer>();
            options.AddSchemaTransformer<IgnoreFieldSchemaTransformer>();
            options.AddCsrfTokenTransformer();
            options.AddClearServerDocumentTransformer();
        });

        //api description provider
        services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, MetadataApiDescriptionProvider>());
    }

    private static void ConfigureMobileApi(IServiceCollection services)
    {
        services.AddOpenApi(ApiConstants.ApiGroupNameV3, options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
            options.AddContactDocumentTransformer("GrandNode2 Mobile API", ApiConstants.ApiGroupNameV3);
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddSchemaTransformer<EnumSchemaTransformer>();
            options.AddClearServerDocumentTransformer();
        });
    }
}
