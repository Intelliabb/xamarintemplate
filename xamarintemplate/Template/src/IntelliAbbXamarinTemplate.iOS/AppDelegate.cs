using System;
using System.Collections.Generic;
using System.Linq;
using Prism;
using Prism.Ioc;

using Foundation;
using UIKit;
using IntelliAbbXamarinTemplate.Services;
using IntelliAbbXamarinTemplate.iOS.Services;
using IntelliAbbXamarinTemplate;

namespace IntelliAbbXamarinTemplate.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new IosInitializer()));
            return base.FinishedLaunching(app, options);
        }
    }

    internal class IosInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFileService, Services.FileService>();
        }
    }
}
