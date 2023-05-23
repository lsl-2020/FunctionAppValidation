using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Trace;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
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
    })
    .Build();

host.Run();