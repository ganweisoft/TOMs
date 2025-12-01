using System;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCenterCore.BackgroundTasks
{
    public interface IBackgroundTask
    {
        Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }
}
