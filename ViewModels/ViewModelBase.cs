using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVVM.Core.Utils;
using UniRx;

namespace MVVM.Core.ViewModels
{
    public interface IViewModel
    {
        Task<bool> InitializeAsync(object data = null);
    }

    public abstract class ViewModelBase : IDisposable, IViewModel
    {
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();
        
        public bool Initialized { get; private set; }
        
        protected object Data { get; private set; }

        public virtual void Dispose()
        {
            if(Disposables != null && Disposables.Count > 0)
                Disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        protected bool Initialize(object data = null)
        {
            var res = false;
            Task.Run(async () => res = await ((IViewModel) this).InitializeAsync(data)).GetAwaiter().GetResult();
            return res;
        }

        async Task<bool> IViewModel.InitializeAsync(object data)
        {
            Data = data;
            if(await OnInitializeAsync() == false) return false;
            Initialized = true;
            return true;
        }
        
        protected Task<bool> InitializeAsync(object data)
        {
            return ((IViewModel) this).InitializeAsync(data);
        }

        protected virtual Task<bool> OnInitializeAsync()
        {
            return Task.FromResult(true);
        }

        protected static async Task<TViewModel[]> TryCreateViewModelsFromDataAsync<TViewModel, TData>(IEnumerable<TData> data, Func<TViewModel> vmFactory)
            where TViewModel : ViewModelBase<TData>
        {
            var vms = data.Select(p => vmFactory.Invoke()).ToArray();
            var loadOps = await Task.WhenAll(data.Zip(vms, (p, c) => c.InitializeAsync(p)));
            return loadOps.Any(r => r == false) ? null : vms;
        }

        protected static async Task<TViewModel[]> TryCreateViewModelsFromDataAsync<TViewModel, TData>(IEnumerable<TData> data, Func<TData, TViewModel> vmFactory)
            where TViewModel : SimpleViewModel
        {
            var vms = data.Select(vmFactory.Invoke).ToArray();
            var loadOps = await Task.WhenAll(vms.Select(vm => vm.InitializeAsync()));
            return loadOps.Any(r => r == false) ? null : vms;
        }
    }

    public abstract class SimpleViewModel : ViewModelBase
    {
        public Task<bool> InitializeAsync()
        {
            return base.InitializeAsync(null);
        }
        
        public bool Initialize()
        {
            return base.Initialize();
        }
    }

    public abstract class ViewModelBase<TData> : ViewModelBase
    {
        protected new TData Data => (TData) base.Data;

        public Task<bool> InitializeAsync(TData data)
        {
            return base.InitializeAsync(data);
        }
        
        public bool Initialize(TData data)
        {
            return base.Initialize(data);
        }
    }
}
