using System;
using System.Threading.Tasks;
using DryIocAttributes;
using UniRx;
using UniRx.Async;

namespace MVVM.Core.Services
{
    public interface IHybridMessageBroker
    {
        IDisposable Subscribe<T>(Action<T> onNext);
        Task PublishAsync<T>(T message);
        IDisposable SubscribeAsync<T>(Func<T, IObservable<Unit>> asyncMessageReceiver);
    }

    [ExportMany, CurrentScopeReuse]
    public class HybridMessageBroker : IHybridMessageBroker
    {
        private readonly MessageBroker _messageBroker;
        private readonly AsyncMessageBroker _asyncMessageBroker;

        public HybridMessageBroker()
        {
            _messageBroker = new MessageBroker();
            _asyncMessageBroker = new AsyncMessageBroker();
        }

        public IDisposable SubscribeAsync<T>(Func<T, IObservable<Unit>> asyncMessageReceiver)
        {
            return _asyncMessageBroker.Subscribe(asyncMessageReceiver);
        }

        public IDisposable Subscribe<T>(Action<T> onNext)
        {
            return _messageBroker.Receive<T>().Subscribe(onNext);
        }

        public Task PublishAsync<T>(T message)
        {
            _messageBroker.Publish(message);
            return _asyncMessageBroker.PublishAsync(message).ToTask();
        }
    }
}