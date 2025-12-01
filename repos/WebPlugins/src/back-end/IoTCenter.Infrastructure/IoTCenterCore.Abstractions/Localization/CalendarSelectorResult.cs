using System;
using System.Threading.Tasks;

namespace IoTCenterCore.Localization
{
    public class CalendarSelectorResult
    {
        public int Priority { get; set; }

        public Func<Task<CalendarName>> CalendarName { get; set; }
    }
}
