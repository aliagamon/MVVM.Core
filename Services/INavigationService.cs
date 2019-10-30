using System.Threading.Tasks;

namespace MVVM.Core.Services
{
    public interface INavigationService
    {
        Task NavigateTo(string screenName, object param = null);
        Task NavigateToEntry(object param = null);
    }
}