# Docker Health Check Configuration

This file contains configuration for adding a health check endpoint to GrandNode2.

## Adding Health Check to Program.cs

Add the following code to your Program.cs file (in the Web project) after the line `StartupBase.ConfigureServices(builder.Services, builder.Configuration);`:

```csharp
// Add health checks for Docker
builder.Services.AddHealthChecks()
    .AddCheck("mongodb", () =>
    {
        try
        {
            var connectionString = builder.Configuration["ConnectionStrings:Mongodb"];
            if (string.IsNullOrEmpty(connectionString))
                return HealthCheckResult.Unhealthy("MongoDB connection string not configured");
            
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);
            database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            return HealthCheckResult.Healthy("MongoDB is available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is unavailable", ex);
        }
    })
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"));
```

And after the line `StartupBase.ConfigureRequestPipeline(app, builder.Environment);`, add:

```csharp
// Configure health check endpoint for Docker
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Basic health check endpoint
app.MapGet("/health", () => "OK");
```

## Required NuGet Packages

Add these packages to your Grand.Web.csproj file:

```xml
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="9.0.0" />
<PackageReference Include="AspNetCore.HealthChecks.UI.Client" Version="8.0.1" />
```

## Alternative Simple Health Check

If you prefer a simpler approach, just add this to Program.cs:

```csharp
// Simple health check endpoint
app.MapGet("/health", () => "OK");
```

This provides a basic endpoint that Docker can use to check if the application is responsive.