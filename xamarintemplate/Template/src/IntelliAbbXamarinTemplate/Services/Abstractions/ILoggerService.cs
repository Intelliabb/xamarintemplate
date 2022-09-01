using System;
using System.Runtime.CompilerServices;

namespace IntelliAbbXamarinTemplate.Services
{
    public interface ILoggerService
    {
        void Info(string message, [CallerMemberName] string caller = null);
        void Error(string errorMessage, [CallerMemberName] string caller = null);
        void Error(Exception ex, [CallerMemberName] string caller = null);
        void Error(string errorMessage, Exception ex, [CallerMemberName] string caller = null);
        void Log(string eventName, string message = null, [CallerMemberName] string caller = null);
    }
}
