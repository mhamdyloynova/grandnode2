using Grand.Infrastructure;
using Grand.Module.MobileApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grand.Module.MobileApi.Infrastructure;

public class MobileApiStartup : IStartupApplication
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register Mobile API configuration
        var mobileApiConfig = new MobileApiConfig();
        configuration.GetSection("MobileAPI").Bind(mobileApiConfig);
        services.AddSingleton(mobileApiConfig);

        if (!mobileApiConfig.Enabled)
            return;

        // Configure MVC to use Newtonsoft.Json globally with DefaultContractResolver
        // This sets PascalCase for all controllers (matching System.Text.Json behavior)
        services.PostConfigure<MvcNewtonsoftJsonOptions>(options =>
        {
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        });
    }

    public void Configure(WebApplication application, IWebHostEnvironment webHostEnvironment)
    {
        // Mobile API specific middleware configuration will go here
        // For now, authentication is handled by the main application
    }

    public int Priority => 500; // After main API module but before OpenAPI
    public bool BeforeConfigure => false;
}