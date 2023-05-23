using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using System;

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
                    // Ensure the TracerProvider subscribes to any custom ActivitySources.
                    builder
                        .AddSource("Azure.*")
                        .SetSampler(new AlwaysOnSampler())
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddConsoleExporter();
                });

            Console.WriteLine("********************** Executed");
        }
    }
}
