using System.Reflection;
using FlaUIServer.Models;
using FlaUIServer.Session;
using Swashbuckle.AspNetCore.Filters;

namespace FlaUIServer.Extensions;

public static class WebApplicationBuilderExtension
{
    /// <summary>
    /// Register services to application builder
    /// </summary>
    /// <param name="builder">Application builder</param>
    /// <param name="options">Server options</param>
    public static void AddServices(this WebApplicationBuilder builder, ServerOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        
        // Configuration
        builder.Services.AddSingleton(options);
        
        // Additional services
        builder.Services.AddSingleton<ISessionManager, SessionManager>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x => x.ExampleFilters());
        builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        
        // Register Mediatr services
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
    }
}