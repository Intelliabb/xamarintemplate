using System;
using System.Runtime.CompilerServices;
using static System.Diagnostics.Debug;
namespace IntelliAbbXamarinTemplate.Services
{
    public class LoggerService : ILoggerService
    {
        const string TAG = "[App]";

        public void Info(string message, [CallerMemberName] string caller = null) =>
            WriteLine($"[{TAG}] [{caller}] [DEBUG] - {message}");

        public void Error(string errorMessage, [CallerMemberName] string caller = null) =>
            WriteLine($"[{TAG}] [{caller}] [ERROR] - {errorMessage}");

        public void Error(string errorMessage, Exception ex, [CallerMemberName] string caller = null) =>
            WriteLine($"[{TAG}] [{caller}] [ERROR] - {errorMessage}\n{ex.GetType().Name}: {ex}");

        public void Error(Exception ex, [CallerMemberName] string caller = null) =>
            WriteLine($"[{TAG}] [{caller}] [ERROR] - {ex.GetType().Name}: {ex}");

        public void Log(string eventName, string message = null, [CallerMemberName] string caller = null)
        {
            WriteLine($"[{TAG}] [{caller}] [INFO] - {eventName}: {message}");
        }
    }
}
