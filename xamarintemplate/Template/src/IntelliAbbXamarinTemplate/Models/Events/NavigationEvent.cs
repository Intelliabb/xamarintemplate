using System;
using Prism.Events;

namespace IntelliAbbXamarinTemplate.Models.Events
{
    public class NavigationEvent : PubSubEvent<NavigationEventArgs>
    {
    }

    public class NavigationEventArgs : EventArgs
    {
        public string Route { get; set; }
        public NavigationEventArgs(string route)
        {
            Route = route;
        }
    }
}
