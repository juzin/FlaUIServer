using System.Reflection;
using FlaUIServer.Models;
using FlaUIServer.Session;
using Swashbuckle.AspNetCore.Filters;

namespace FlaUIServer.Extensions;

public static class WebApplicationBuilderExtension
{
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
        
        // Register MediatR services
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
    }
}