using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using IntelliAbbXamarinTemplate.Constants;

namespace IntelliAbbXamarinTemplate.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            // Good spot to initialize notification mechanism
        }
    }
}
