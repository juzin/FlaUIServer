using FlaUIServer.Extensions;
using FlaUIServer.Helpers;
using FlaUIServer.Middlewares;
using Serilog;

// --urls= --cleanup-cycle=90 --use-swagger --use-basic-auth --allow-powershell --log-response-body
var options = CommandLineArgumentsHelper.ParseArguments(args);
var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add required services to the container
builder.AddServices(options);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || options.UseSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    options.UseSwagger = true;
}

// Use request/response body logging middleware
if (options.LogResponseBody)
{
    app.UseMiddleware<RequestLoggingMiddleware>();
}
else
{
    app.UseSerilogRequestLogging();
}

// Basic authentication middleware
if (options.UseBasicAuthentication)
{
    app.UseMiddleware<BasicAuthorizationMiddleware>();
}

// Map minimal api endpoints
app.MapGroup("/wd/hub")
    .MapSessionEndpoints()
    .WithTags("WinApp Session");

app.MapGroup("")
    .MapServerInfoEndpoint()
    .WithTags("Server Information");

// Log server start
ServerStartConsoleHelper.WriteLogo();
ServerStartConsoleHelper.LogServerOptions(app.Logger, options);

await app.RunAsync();