using System.Threading.Tasks;
using MVVM.Core.ViewModels;

namespace MVVM.Core.Services
{
    public interface IScreenController
    {
        Task HideScreen();
        Task UnHideScreen();
        Task PrepareScreen(ScreenViewModelBase viewModel);
    }
}
