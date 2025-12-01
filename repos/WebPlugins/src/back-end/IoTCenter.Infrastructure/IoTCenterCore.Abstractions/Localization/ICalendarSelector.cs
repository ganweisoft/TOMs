using System.Threading.Tasks;

namespace IoTCenterCore.Localization
{
    public interface ICalendarSelector
    {
        Task<CalendarSelectorResult> GetCalendarAsync();
    }
}
