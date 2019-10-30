using System;
using System.Collections.Generic;
using DryIoc;
using DryIocAttributes;
using IfUnresolved = DryIoc.IfUnresolved;
using Request = DryIoc.Request;

namespace MVVM.Core.Services
{
    public interface IScopeManager : IResolver
    {
        void OpenScope(string name = null);
        string ScopeName { get; }
        bool IsRegistered(Type type);
    }

    [ExportMany, SingletonReuse]
    public class ScopeManager : IScopeManager
    {
        private readonly IContainer _container;

        public ScopeManager(IContainer container)
        {
            _container = container;
        }

        private IResolverContext _scope;
        private IResolverContext Context => _scope ?? _container;

        public void OpenScope(string name = null)
        {
            _scope?.Dispose();
            _scope = _container.OpenScope(name);
        }

        public string ScopeName => _scope?.CurrentScope?.Name.ToString();

        public bool IsRegistered(Type type)
        {
            return _container.IsRegistered(type);
        }

        public object Resolve(Type serviceType, IfUnresolved ifUnresolved)
        {
            return Context.Resolve(serviceType, ifUnresolved);
        }

        public object Resolve(Type serviceType, object serviceKey, IfUnresolved ifUnresolved, Type requiredServiceType,
            Request preResolveParent, object[] args)
        {
            return Context.Resolve(serviceType, serviceKey, ifUnresolved, serviceType, preResolveParent, args);
        }

        public IEnumerable<object> ResolveMany(Type serviceType, object serviceKey, Type requiredServiceType, Request preResolveParent, object[] args)
        {
            return Context.ResolveMany(requiredServiceType, serviceKey, requiredServiceType, preResolveParent, args);
        }

        public object GetService(Type serviceType)
        {
            return Context.Resolve(serviceType);
        }
    }
}