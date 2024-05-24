using System.Threading.Tasks;
using System.Threading;
using System;

namespace Rsbc.Dmf.IcbcAdapter.BackgroundWorkItem
{
    /// <summary>
    ///
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken);
        /// <summary>
        ///
        /// </summary>
        /// <param name="workItem"></param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    }
}
