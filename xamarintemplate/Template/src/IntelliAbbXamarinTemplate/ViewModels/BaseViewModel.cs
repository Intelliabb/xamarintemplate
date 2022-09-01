using System;
using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using IntelliAbbXamarinTemplate.Models;
using Prism.AppModel;
using Prism.Regions.Navigation;
using IntelliAbbXamarinTemplate.Services;

namespace IntelliAbbXamarinTemplate.ViewModels
{
    public class BaseViewModel : BindableBase,
                                INavigationAware,
                                IInitialize,
                                IApplicationLifecycleAware,
                                IDisposable
    {
        protected ILoggerService LoggerService { get; }

        public BaseViewModel(ILoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters) { }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters != null && parameters.GetNavigationMode() != NavigationMode.Back)
            {
                TrackPageVisit();
            }
        }

        public virtual void Initialize(INavigationParameters parameters) { }

        public virtual void OnResume() { }

        public virtual void OnSleep() { }

        //IRegion Aware
        public virtual void OnNavigatedTo(INavigationContext navigationContext)
        {
            TrackPageVisit();
        }

        void TrackPageVisit()
        {
            var page = ToString();
            try
            {
                var name = page[(page.LastIndexOf('.') + 1)..^9];
                LoggerService.Log(Constants.Analytics.PageVisit, name);
            }
            catch (Exception ex)
            {
                LoggerService.Log(Constants.Analytics.Warning, $"Error tracking page: {page}. {ex}");
            }
        }
        public virtual bool IsNavigationTarget(INavigationContext navigationContext) => false;

        public virtual void OnNavigatedFrom(INavigationContext navigationContext) { }

        #region Runners

        /// <summary>
        /// Runs the task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="task">Task.</param>
        /// <param name="handleException">If set to <c>true</c> handle exception. Otherwise, throw the exeption to be handled by calling ViewModel.</param>
        /// <param name="actionOnException">If handleException is <c>false</c>, then invoke this method to handle exception.</param>
        /// <param name="messageOnException">Message on display when exception occurs.</param>
        /// <param name="lockNavigation">Lock the navigation and keep the user on the current view.</param>
        /// <param name="ignoreIsBusy">tells the Task Runner that it should not modify the state of Is Busy</param>
        /// <param name="caller">Calling method.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected async Task RunTask(Task task, bool handleException = true, Action<Exception, string> actionOnException = null, string messageOnException = null, bool? lockNavigation = null,
            bool ignoreIsBusy = false, [CallerMemberName] string caller = null)
        {
            UpdateIsBusy(!ignoreIsBusy || IsBusy, lockNavigation);

            await task.ContinueWith((thisTask) => {

                UpdateIsBusy(ignoreIsBusy && IsBusy);

                if (thisTask.IsFaulted && thisTask.Exception != null)
                {
                    Exception exception = null;
                    if (thisTask.Exception.InnerException != null)
                    {
                        exception = thisTask.Exception.InnerException;
                        while (exception.InnerException != null)
                            exception = exception.InnerException;
                    }
                    else
                    {
                        exception = thisTask.Exception;
                    }

                    if (exception is OfflineException)
                    {
                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Offline", "You are offline", "OK");
                            //NavigationService.NavigateAsync(nameof(ServiceInfoPage), new NavigationParameters { { AppConstants.MaintenanceMessageKey, (exception.InnerException ?? exception).Message } }, true, false);
                        });
                    }
                    else if (!handleException)
                    {
                        if (actionOnException == null)
                            throw exception;

                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => actionOnException.Invoke(exception, messageOnException));
                    }
                }
            });
        }

        /// <summary>
        /// Runs the task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="task">Task.</param>
        /// <param name="handleException">If set to <c>true</c> handle exception. Otherwise, throw the exeption to be handled by calling ViewModel.</param>
        /// <param name="actionOnException">If handleException is <c>false</c>, then invoke this method to handle exception.</param>
        /// <param name="messageOnException">Message on display when exception occurs.</param>
        /// <param name="lockNavigation">Lock the navigation and keep the user on the current view.</param>
        /// <param name="ignoreIsBusy">tells the Task Runner that it should not modify the state of Is Busy</param>
        /// <param name="caller">Calling method.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected async Task<T> RunTask<T>(Task<T> task, bool handleException = true, Action<Exception, string> actionOnException = null, string messageOnException = null, bool? lockNavigation = null,
            bool ignoreIsBusy = false, [CallerMemberName] string caller = null)
        {
            try
            {
                await RunTask((Task)task, handleException, actionOnException, messageOnException, lockNavigation, ignoreIsBusy, caller);
                return task.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{(ex.InnerException ?? ex).Message}");
                UpdateIsBusy(false);
                return default(T);
            }
        }

        private void UpdateIsBusy(bool isBusy, bool? lockNavigation = null)
        {
            IsBusy = isBusy;
            //CanNavigate = lockNavigation.HasValue ? !lockNavigation.Value : !isBusy;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
