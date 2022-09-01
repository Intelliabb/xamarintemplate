namespace IntelliAbbXamarinTemplate.Constants
{
    public static class AppConstants
    {
        public const string AppName = "IntelliAbbXamarinTemplate";
        public const int DefaultGetRetryCount = 3;
        public const string ContentTypeJson = "application/json";
    }

    public static class Apis
    {
        public const string SomeEndPoint = "api/";
    }

    public static class CacheKeys
    {
        public const string LastRefreshed = "last_refreshed";
    }

    public static class PreferencesKeys
    {
        public const string AppTheme = "app_theme";
        public const string IsFirstLaunch = "is_first_launch";
        public const string LastAppVersion = "last_app_version";
    }

    public static class NotificationTopics
    {
        public const string General = "general";
        public const string Dev = "general_dev";
    }

    public static class Analytics
    {
        public const string PushNotificationDismissed = "Push Notification Dismissed";
        public const string PageVisit = "Page visits";
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Info = "Info";
    }
}
