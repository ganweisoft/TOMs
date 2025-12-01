using System.Threading.Tasks;

namespace IoTCenterCore.Environment.Shell.Builders
{
    public interface IShellPipeline
    {
        Task Invoke(object context);
    }
}
