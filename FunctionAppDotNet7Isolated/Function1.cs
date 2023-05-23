using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace FunctionAppDotNet7Isolated
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] MyInfo myTimer)
        {
            _logger.LogInformation("C# Timer trigger function executed at: {CurrentDateTime}", DateTime.Now);
            _logger.LogInformation("Next timer schedule at: {NextScheduledTime}", myTimer.ScheduleStatus.Next);
            _logger.LogInformation("Current trace Id: {CurrentTraceId}", Activity.Current?.Id);

            using HttpClient client = new();
            Task<string> stringTask = client.GetStringAsync("https://www.bing.com/");
            string response = await stringTask;
            Console.WriteLine($"HTTP Response: {response.Substring(0, 255)}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
