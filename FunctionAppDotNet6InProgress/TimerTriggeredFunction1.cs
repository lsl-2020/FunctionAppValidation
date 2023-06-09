using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionAppDotNet6InProgress
{
    public class TimerTriggeredFunction1
    {
        [Disable]
        [FunctionName("TimerTriggeredFunction1")]
        public static async Task RunAsync([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("C# Timer trigger function executed at: {CurrentDateTime}", DateTime.Now);
            log.LogInformation("Current trace Id: {CurrentTraceId}", Activity.Current?.Id);

            using HttpClient client = new();
            Task<string> stringTask = client.GetStringAsync("https://www.bing.com/");
            string response = await stringTask;
            Console.WriteLine($"HTTP Response: {response.Substring(0, 255)}");
        }
    }
}