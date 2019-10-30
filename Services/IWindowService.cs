using MVVM.Core.ViewModels;
using UniRx.Async;

namespace MVVM.Core.Services
{
    public interface IWindowService
    {
        void CloseWindow(string windowName);
        void ShowWindow(string windowName);
        UniTask ShowViewAsync(string windowName, ViewModelBase viewModel, bool dialogue = false);
    }
}
