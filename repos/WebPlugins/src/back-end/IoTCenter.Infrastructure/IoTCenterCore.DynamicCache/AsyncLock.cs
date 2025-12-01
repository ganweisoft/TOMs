using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCenterCore.DynamicCache
{
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly Releaser _releaser;
        private readonly Task<Releaser> _releaserTask;

        public AsyncLock() : this(1) { }
        public AsyncLock(int initialCount)
        {
            _semaphore = new SemaphoreSlim(initialCount);
            _releaser = new Releaser(this);
            _releaserTask = Task.FromResult(_releaser);
        }
        public AsyncLock(int initialCount, int maxCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, maxCount);
            _releaser = new Releaser(this);
            _releaserTask = Task.FromResult(_releaser);
        }

        public Task<Releaser> LockAsync(CancellationToken cancellationToken)
        {
            var wait = _semaphore.WaitAsync(cancellationToken);

            return wait.IsCompleted
                ? _releaserTask
                : wait.ContinueWith((_, state) => ((AsyncLock)state)._releaser,
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public Releaser Lock()
        {
            _semaphore.Wait();
            return _releaser;
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            public void Dispose()
            {
                if (_toRelease != null)
                {
                    _toRelease._semaphore.Release();
                }
            }
        }
    }
}
