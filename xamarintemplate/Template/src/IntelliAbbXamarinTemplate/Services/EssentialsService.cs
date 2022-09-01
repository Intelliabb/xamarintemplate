using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace IntelliAbbXamarinTemplate.Services
{
    public interface IEssentialsService
    {
        Task ShareUri(string uri, string title);
        Task<bool> OpenBrowser(string link);
        Task<bool> CanOpenLauncher(string uriString);
        Task<bool> CanOpenLauncher(Uri uri);
        Task<bool> TryOpenLauncher(string uriString);
        Task<bool> TryOpenLauncher(Uri uri);
        Task OpenLauncher(Uri uriString);
        Task OpenLauncher(OpenFileRequest fileRequest);
        Task OpenLauncher(string uriString);
        Task<string> ReadFile(string filePath);
        bool PreferenceExists(string key);
        /// <summary>
        /// Gets the preference for given key. If key does not exist, returns the defaultValue
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="defaultValue">Returned value if the key does not exist</param>
        /// <returns></returns>
        object GetPreference(string key, object defaultValue);
        void SavePreference(string key, object value = null);
        bool IsNetworkAvailable();
        string DevicePlatform { get; }
        string AppVersion { get; }
    }

    public class EssentialsService : IEssentialsService
    {
        public string DevicePlatform => DeviceInfo.Platform.ToString();
        public string AppVersion => AppInfo.VersionString;

        public Task<bool> CanOpenLauncher(string uriString) => Launcher.CanOpenAsync(uriString);
        public Task<bool> CanOpenLauncher(Uri uri) => Launcher.CanOpenAsync(uri);
        public Task<bool> TryOpenLauncher(string uriString) => Launcher.TryOpenAsync(uriString);
        public Task<bool> TryOpenLauncher(Uri uri) => Launcher.TryOpenAsync(uri);
        public Task OpenLauncher(Uri uriString) => Launcher.OpenAsync(uriString);
        public Task OpenLauncher(OpenFileRequest fileRequest) => Launcher.OpenAsync(fileRequest);
        public Task OpenLauncher(string uriString) => Launcher.OpenAsync(uriString);
        public Task ShareUri(string uri, string title) => Share.RequestAsync(new ShareTextRequest(uri, title));
        public bool PreferenceExists(string key) => Preferences.ContainsKey(key);

        public object GetPreference(string key, object defaultValue)
        {
            if (defaultValue is string s)
            {
                return Preferences.Get(key, s);
            }

            if (defaultValue is int dInt)
            {
                return Preferences.Get(key, dInt);
            }

            if (defaultValue is long dLong)
            {
                return Preferences.Get(key, dLong);
            }

            if (defaultValue is double d)
            {
                return Preferences.Get(key, d);
            }

            if (defaultValue is float f)
            {
                return Preferences.Get(key, f);
            }

            if (defaultValue is bool b)
            {
                return Preferences.Get(key, b);
            }
            return defaultValue;
        }

        public void SavePreference(string key, object value)
        {
            if (value is string s)
            {
                Preferences.Set(key, s);
            }

            if (value is int dInt)
            {
                Preferences.Set(key, dInt);
            }

            if (value is long l)
            {
                Preferences.Set(key, l);
            }

            if (value is double d)
            {
                Preferences.Set(key, d);
            }

            if (value is float f)
            {
                Preferences.Set(key, f);
            }

            if (value is bool b)
            {
                Preferences.Set(key, b);
            }
        }

        public Task<bool> OpenBrowser(string link)
        {
            return Browser.OpenAsync(new Uri(link), new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Default
            });
        }

        public bool IsNetworkAvailable()
        {
            return Connectivity.NetworkAccess != NetworkAccess.None &&
                Connectivity.NetworkAccess != NetworkAccess.Unknown &&
                Connectivity.NetworkAccess != NetworkAccess.Local;
        }

        public async Task<string> ReadFile(string filePath)
        {
            string fileContents;
            using (var stream = await FileSystem.OpenAppPackageFileAsync(filePath))
            {
                using var reader = new StreamReader(stream);
                fileContents = await reader.ReadToEndAsync();
            }
            return fileContents;
        }
    }
}
