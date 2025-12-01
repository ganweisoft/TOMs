using System.Threading.Tasks;

namespace IoTCenterCore.Modules
{
    public interface ITimeZoneSelector
    {
        Task<TimeZoneSelectorResult> GetTimeZoneAsync();
    }
}
