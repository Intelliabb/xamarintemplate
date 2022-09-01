using Prism.Mvvm;

namespace IntelliAbbXamarinTemplate.Models
{
    public class NotificationItem : BindableBase
    {
        private object _value;
        public string Code { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}