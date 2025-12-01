using Microsoft.Extensions.Primitives;
using IoTCenterCore.Environment.Shell.Scope;

namespace IoTCenterCore.Environment.Cache
{
    public interface ISignal
    {
        IChangeToken GetToken(string key);

        void SignalToken(string key);
    }

    public static class SignalExtensions
    {
        public static void DeferredSignalToken(this ISignal signal, string key)
        {
            ShellScope.AddDeferredSignal(key);
        }
    }
}
