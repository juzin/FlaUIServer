using System.Reflection;
using FlaUIServer.Session;
using Swashbuckle.AspNetCore.Filters;

namespace FlaUIServer.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Additional services
        builder.Services.AddSingleton<ISessionManager, SessionManager>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => options.ExampleFilters());
        builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        
        // Register MediatR services
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
    }
}