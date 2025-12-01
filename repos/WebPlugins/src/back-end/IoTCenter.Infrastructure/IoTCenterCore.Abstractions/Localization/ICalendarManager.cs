using System.Threading.Tasks;

namespace IoTCenterCore.Localization
{
    public interface ICalendarManager
    {
        Task<CalendarName> GetCurrentCalendar();
    }
}
