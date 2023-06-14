using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiSharp
{
    public class SingleThreadSynchronizationContext : SynchronizationContext
    {
        readonly BlockingCollection<Action> _queue = new();

        public override void Post(SendOrPostCallback d, object? state)
        {
            _queue.Add(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            throw new NotImplementedException();
        }

        public void InvokeCallbacks()
        {
            while (_queue.TryTake(out Action? a, Timeout.Infinite))
                a?.Invoke();
        }

        public void Complete()
        {
            _queue.CompleteAdding();
        }

        public static void Run(Func<Task> func)
        {
            var prevCtx = Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext();
                SetSynchronizationContext(syncCtx);

                Task main = func();
                main.ContinueWith(t => syncCtx.Complete(), TaskScheduler.Default);

                syncCtx.InvokeCallbacks();

                main.GetAwaiter().GetResult();
            }
            finally { SetSynchronizationContext(prevCtx); }
        }
    }
}
