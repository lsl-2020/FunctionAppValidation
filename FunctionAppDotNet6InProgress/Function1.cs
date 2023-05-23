using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionAppDotNet6InProgress
{
    public class Function1
    {
        [FunctionName("Function1")]
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
