using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BnetworkManagement.Extensions
{
    public class AsyncLocker<T>
    {
        private LazyDictionary<T, SemaphoreSlim> semaphoreDictionary =
            new LazyDictionary<T, SemaphoreSlim>();

        public async Task<IDisposable> LockAsync(T key)
        {
            var semaphore = semaphoreDictionary.GetOrAdd(key, () => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            return new ActionDisposable(() => semaphore.Release());
        }
    }
}
