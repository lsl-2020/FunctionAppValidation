// Ignore Spelling: Aspnetcore

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FunctionAppDotNet6InProgress
{
    public class AspnetcoreEventSourceListener : EventListener
    {
        private readonly List<EventSource> _eventSources = new();
        private readonly Action<EventWrittenEventArgs, string> _log;
        private readonly EventLevel _level;

        private HashSet<string> _eventSourceNames = new HashSet<string>();

        public AspnetcoreEventSourceListener(Action<EventWrittenEventArgs, string> log, EventLevel level)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _level = level;

            foreach (EventSource eventSource in _eventSources)
            {
                OnEventSourceCreated(eventSource);
            }

            _eventSources.Clear();
            _eventSourceNames.Clear();
        }

        protected sealed override void OnEventSourceCreated(EventSource eventSource)
        {
            base.OnEventSourceCreated(eventSource);

            _eventSourceNames?.Add(eventSource.Name);

            if (_log == null)
            {
                _eventSources.Add(eventSource);
            }

            if (eventSource.Name == "Microsoft-Diagnostics-DiagnosticSource")
            //if (eventSource.Name == "Microsoft.AspNetCore.Hosting")
            //if (eventSource.Name == "System.Net.Http")
            {
                EnableEvents(eventSource, _level);
            }
        }

        public static AspnetcoreEventSourceListener CreateConsoleLogger(EventLevel level = EventLevel.Informational)
        {
            return new AspnetcoreEventSourceListener((eventData, text) => Console.WriteLine("[{1}] {0}: {2}", eventData.EventSource.Name, eventData.Level, text), level);
        }

        protected sealed override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // Workaround https://github.com/dotnet/corefx/issues/42600
            if (eventData.EventId == -1)
            {
                return;
            }

            // There is a very tight race during the AzureEventSourceListener creation where EnableEvents was called
            // and the thread producing events not observing the `_log` field assignment
            if (eventData?.Payload != null)
            {
                foreach (var pl in eventData.Payload)
                {
                    if (pl?.ToString()?.Contains("Microsoft.AspNetCore.Hosting.HttpRequestIn") == true)
                    {
                        _log?.Invoke(eventData, Format(eventData));
                        break;
                    }
                }
            }
        }

        private string Format(EventWrittenEventArgs eventData)
        {
            var payloadArray = eventData.Payload?.ToArray() ?? Array.Empty<object?>();

            for (int i = 0; i < payloadArray.Length; i++)
            {
                payloadArray[i] = FormatValue(payloadArray[i]);
            }

            if (eventData.Message != null)
            {
                try
                {
                    return string.Format(CultureInfo.InvariantCulture, eventData.Message, payloadArray);
                }
                catch (FormatException)
                {
                }
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(eventData.EventName);

            if (!string.IsNullOrWhiteSpace(eventData.Message))
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(nameof(eventData.Message)).Append(" = ").Append(eventData.Message);
            }

            if (eventData.PayloadNames != null)
            {
                for (int i = 0; i < eventData.PayloadNames.Count; i++)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(eventData.PayloadNames[i]).Append(" = ").Append(payloadArray[i]);
                }
            }

            //Console.WriteLine(stringBuilder.ToString());

            return stringBuilder.ToString();
        }

        private static object? FormatValue(object? o)
        {
            if (o is byte[] bytes)
            {
                var stringBuilder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
                }

                return stringBuilder.ToString();
            }

            return o;
        }
    }
}
