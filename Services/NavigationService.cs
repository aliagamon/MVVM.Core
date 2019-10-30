using System;
using System.Threading.Tasks;
using DryIocAttributes;
using MVVM.Core.ViewModels;
using IfUnresolved = DryIoc.IfUnresolved;

namespace MVVM.Core.Services
{
    public interface IEntryScreenNameProvider
    {
        string EntryScreen { get; }
    }
    
    [ExportMany, SingletonReuse]
    public class NavigationService : INavigationService
    {
        private readonly ILogger _logger;
        private readonly IScreenToViewModelMapper _mapper;
        private readonly IScreenController _router;
        private readonly IScopeManager _scopeManager;
        private ScreenViewModelBase _currentScreen;
        private string _inLoadingScreenName;
        private readonly IEntryScreenNameProvider _entryScreenNameProvider;

        public NavigationService(
            ILogger logger, 
            IScreenController router, 
            IScopeManager scopeManager,
            IScreenToViewModelMapper mapper,
            IEntryScreenNameProvider entryScreenNameProvider
            )
        {
            _entryScreenNameProvider = entryScreenNameProvider;
            _mapper = mapper;
            _scopeManager = scopeManager;

            _router = router;
            _logger = logger;
        }

        public async Task NavigateTo(string screenName, object param = null)
        {
            if (_currentScreen?.ScreenName == screenName || _inLoadingScreenName == screenName)
                return;
            _inLoadingScreenName = screenName;

            var viewModelType = _mapper.GetViewModelType(screenName);
            if (viewModelType == null)
            {
                _logger.LogError($"no screen view model found for {screenName}");
                Relogin();
                return;
            }

            if (_currentScreen != null)
                await _router.HideScreen();
            _scopeManager.OpenScope(screenName);

            var nextScreen = (ScreenViewModelBase) _scopeManager.Resolve(viewModelType, IfUnresolved.Throw);
            try
            {
                await ((IViewModel)nextScreen).InitializeAsync(param);
                if (_currentScreen != null)
                {
                    await _currentScreen.OnNavigatingFrom();
                    _currentScreen.Dispose();
                }

                _currentScreen = nextScreen;
                await _router.PrepareScreen(nextScreen);
                _inLoadingScreenName = null;
                await _currentScreen.OnNavigatedTo();
            }
            catch (Exception e)
            {
                _logger.LogError($"NavigationService: {viewModelType.Name} InitializeAsync failed with exception: {e}");
                nextScreen.Dispose();
                await _router.UnHideScreen();
                throw;
            }
        }

        public Task NavigateToEntry(object param = null)
        {
            return NavigateTo(_entryScreenNameProvider.EntryScreen, param);
        }


        private async void Relogin()
        {
            await NavigateTo(_entryScreenNameProvider.EntryScreen);
        }
    }
}