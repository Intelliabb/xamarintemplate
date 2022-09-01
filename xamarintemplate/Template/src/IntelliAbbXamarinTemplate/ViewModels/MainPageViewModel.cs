using System;
using System.Linq;
using System.Threading.Tasks;
using ImTools;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Regions.Navigation;
using IntelliAbbXamarinTemplate.Constants;
using IntelliAbbXamarinTemplate.Models;
using IntelliAbbXamarinTemplate.Services;
using IntelliAbbXamarinTemplate.Views;
using Xamarin.Forms;

namespace IntelliAbbXamarinTemplate.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        readonly INavigationService _navigationService;
        
        public MainPageViewModel(INavigationService navigationService, ILoggerService loggerService) : base(loggerService)
        {
            _navigationService = navigationService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            await RunTask(LoadPage()).ConfigureAwait(false);
        }

        Task LoadPage()
        {
            Title = "Main Page";
            return Task.CompletedTask;
        }
    }
}
