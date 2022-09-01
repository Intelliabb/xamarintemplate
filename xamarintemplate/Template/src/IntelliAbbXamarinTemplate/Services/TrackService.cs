using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AppCenter.Crashes;

namespace IntelliAbbXamarinTemplate.Services
{
    public class TrackService : ILoggerService
    {
        public void Info(string message, [CallerMemberName] string caller = null)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(Constants.Analytics.Info, new Dictionary<string, string>
            {
                { "caller", caller},
                { "message", message }
            });
        }

        public void Error(string errorMessage, [CallerMemberName] string caller = null)
        {
            Crashes.TrackError(new Exception(errorMessage, null), new Dictionary<string, string> { { "caller", caller } });
        }

        public void Error(string errorMessage, Exception ex, [CallerMemberName] string caller = null)
        {
            Crashes.TrackError(ex, new Dictionary<string, string> { { "caller", caller }, { "message", errorMessage } });
        }

        public void Error(Exception ex, [CallerMemberName] string caller = null)
        {
            Crashes.TrackError(ex, new Dictionary<string, string> { { "caller", caller } });
        }

        public void Log(string eventName, string message = "", [CallerMemberName] string caller = null)
        {
            var props = new Dictionary<string, string>
            {
                { "caller", caller }
            };

            if (!string.IsNullOrWhiteSpace(message))
                props.Add("message", message);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, props);
        }
    }
}
