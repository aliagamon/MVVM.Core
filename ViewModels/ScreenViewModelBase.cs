using System.Threading.Tasks;
using MVVM.Core.Services;
using MVVM.Core.ViewModels.Requests;
using UniRx;
using UniRx.Async;

namespace MVVM.Core.ViewModels
{
    public abstract class ScreenViewModelBase : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        protected readonly IHybridMessageBroker MessageBroker;

        protected ScreenViewModelBase(INavigationService navigationService, IHybridMessageBroker messageBroker)
        {
            MessageBroker = messageBroker;
            _navigationService = navigationService;
        }

        public abstract AsyncReactiveCommand BackToHomeCommand { get; }

        public abstract string ScreenName { get; }

        protected async Task BackToHome()
        {
            await _navigationService.NavigateToEntry();
        }

        protected async UniTask NavigateTo(string screenName, object param = null)
        {
            await _navigationService.NavigateTo(screenName, param);
        }

        protected new Task<bool> InitializeAsync(object data = null)
        {
            return base.InitializeAsync(data);
        }

        protected override Task<bool> OnInitializeAsync()
        {
            BackToHomeCommand.Subscribe(unit => BackToHome().ToObservable()).AddTo(Disposables);
            MessageBroker.SubscribeAsync<ReturnToMenuRequest>(request => BackToHome().ToObservable()).AddTo(Disposables);
            return base.OnInitializeAsync();
        }

        public virtual Task OnNavigatingFrom()
        {
            return Task.CompletedTask;
        }
        public virtual Task OnNavigatedTo()
        {
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            BackToHomeCommand?.Dispose();
        }
    }

    public abstract class SimpleScreenViewModel : ScreenViewModelBase
    {
        protected SimpleScreenViewModel(INavigationService navigationService, IHybridMessageBroker messageBroker)
            : base(navigationService, messageBroker)
        {
        }

        public Task<bool> InitializeAsync()
        {
            return base.InitializeAsync();
        }
    }

    public abstract class ScreenViewModelBase<TData> : ScreenViewModelBase
    {
        protected ScreenViewModelBase(INavigationService navigationService, IHybridMessageBroker messageBroker)
            : base(navigationService, messageBroker)
        {
        }

        protected new TData Data => (TData) base.Data;

        public Task<bool> InitializeAsync(TData data)
        {
            if (data == null) return Task.FromResult(false);
            return base.InitializeAsync(data);
        }
    }
}