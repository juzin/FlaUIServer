using System.Reflection;
using FlaUIServer.Models;
using FlaUIServer.Services;
using FlaUIServer.Session;
using Microsoft.OpenApi.Models;
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

        // Session manager
        builder.Services.AddSingleton<ISessionManager, SessionManager>();

        // Basic authentication configuration
        if (options.UseBasicAuthentication)
        {
            builder.Services.Configure<BasicAuthenticationConfiguration>(
                builder.Configuration.GetSection(BasicAuthenticationConfiguration.BasicAuthentication));
        }
        
        // Add swagger services
        if (builder.Environment.IsDevelopment() || options.UseSwagger)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.ExampleFilters();

                // Authentication
                if (options.UseBasicAuthentication)
                {
                    x.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "basic",
                        In = ParameterLocation.Header,
                        Description = "Basic Authorization header."
                    });
                    x.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                }
            });
            builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        }

        // Register Mediatr services
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        // Inactive session cleanup
        if (options.SessionCleanupCycleSeconds != 0)
        {
            builder.Services.AddHostedService<SessionCleanupService>();
        }
    }
}