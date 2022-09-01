using System;
using System.Threading.Tasks;
using IntelliAbbXamarinTemplate.Data;
using IntelliAbbXamarinTemplate.Helpers;
using IntelliAbbXamarinTemplate.Models;
using IntelliAbbXamarinTemplate.Services;
using IntelliAbbXamarinTemplate.ViewModels;
using IntelliAbbXamarinTemplate.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Ioc;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IntelliAbbXamarinTemplate
{
    public partial class App
    {
        private ICacheService _cacheService;
        private IEssentialsService _essentialsService;
        private ILoggerService _loggerService;

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            LogUnobservedTaskExceptions();
            SubsribeEvents();
            await NavigationService.NavigateAsync(nameof(MainPage), null, null, animated: false);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterRegionServices();

            // Pages
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            // Services
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<INetworkService, NetworkService>();
            containerRegistry.Register<IDatabase, Database>();
            containerRegistry.Register<ICacheRepository, CacheRepository>();

            // Singletons
#if DEBUG
            containerRegistry.RegisterSingleton<ILoggerService, LoggerService>();
#else
            containerRegistry.RegisterSingleton<ILoggerService, TrackService>();
#endif
            containerRegistry.RegisterSingleton<IEssentialsService, EssentialsService>();
            containerRegistry.RegisterSingleton<IMemoryCache, MemoryCache>();
            containerRegistry.RegisterSingleton<IDeviceCache, DeviceCache>();
            containerRegistry.RegisterSingleton<ICacheService, CacheService>();
        }

        bool IsNewerVersion(string last, string current)
        {
            var lastVersion = new Version(last);
            var currentVersion = new Version(current);
            return currentVersion > lastVersion;
        }

        void InitializeAppCenter()
        {
            /*
            * TODO: To enable secrets, 
            * 1. create a file named secrets.json in the project root
            * 2. build the project. Now you have access to all the secrets in the json file in the static Secrets class
            */

            // var appId = _essentialsService.DevicePlatform == Xamarin.Forms.Device.Android
            //     ? $"android={Secrets.AppCenterAndroidSecret};"
            //     : $"ios={Secrets.AppCenterIosSecret};";

            // AppCenter.Start(appId, typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Crashes));
        }

        void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                _loggerService.Error(e.Exception.Message, e.Exception);
            };
        }

        void SubsribeEvents()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        void UnsubsribeEvents()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        void Connectivity_ConnectivityChanged(object sender, Xamarin.Essentials.ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None ||
                Connectivity.NetworkAccess == NetworkAccess.Unknown ||
                Connectivity.NetworkAccess == NetworkAccess.Local)
            {
                MainPage?.DisplayToastAsync("Connection lost. Try again later.");
            }
        }

        protected override void CleanUp()
        {
            UnsubsribeEvents();
            base.CleanUp();
        }
    }
}
