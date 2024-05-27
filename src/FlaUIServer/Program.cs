using FlaUIServer.Extensions;
using FlaUIServer.Helpers;
using FlaUIServer.Middlewares;
using FlaUIServer.Services;
using Serilog;

// TODO: Command line configuration
// Urls, if not specified map all interfaces default port 4723
// Enable disable swagger
// Enable disable request/response body logging
// Specify url base path where default is /wd/hub

var builder = WebApplication.CreateBuilder(args);
// Configure logging
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add required services to the container
builder.AddServices();
builder.Services.AddHostedService<OrphanedSessionCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use request/response body logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();
// Or use app.UseSerilogRequestLogging()

// Map minimal api endpoints
app.MapGroup("/wd/hub")
    .MapSessionEndpoints()
    .WithTags("WinApp Session");

app.MapGroup("")
    .MapServerInfoEndpoint()
    .WithTags("Server Information");

// Log server start
ServerStartConsoleHelper.WriteLogo();
app.Logger.LogInformation("Starting FlaUI Server, listening at url: {Urls}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));

app.Run();