using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Trace;

[assembly: FunctionsStartup(typeof(FunctionAppDotNet6InProgress.Startup))]

namespace FunctionAppDotNet6InProgress
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .AddSource("Azure.*")
                        .SetSampler(new AlwaysOnSampler())
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddConsoleExporter();
                });
            AspnetcoreEventSourceListener.CreateConsoleLogger();
        }
    }
}