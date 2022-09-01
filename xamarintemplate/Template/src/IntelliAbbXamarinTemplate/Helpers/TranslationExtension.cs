using System;
using System.Globalization;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IntelliAbbXamarinTemplate.Helpers
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslationExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            ResourceManager resmgr = new ResourceManager(typeof(Localization.en_us));

            var translation = resmgr.GetString(Text, CultureInfo.CurrentCulture);

            if (translation == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Key '{Text}' was not found in resources for culture '{CultureInfo.CurrentCulture.Name}'.");
#endif
                translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
            }

            return translation;
        }
    }
}
