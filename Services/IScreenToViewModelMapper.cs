using System;

namespace MVVM.Core.Services
{
    public interface IScreenToViewModelMapper
    {
        Type GetViewModelType(string name);
    }
}