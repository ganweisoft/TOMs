using System;
using System.Linq;
using NodaTime;
using NodaTime.TimeZones;

namespace IoTCenterCore.Modules
{
    public class Clock : IClock
    {
        private static Instant CurrentInstant => SystemClock.Instance.GetCurrentInstant();

        public DateTime UtcNow => CurrentInstant.ToDateTimeUtc();

        public ITimeZone[] GetTimeZones()
        {
            var list =
                from location in TzdbDateTimeZoneSource.Default.ZoneLocations
                let zoneId = location.ZoneId
                let tz = DateTimeZoneProviders.Tzdb[zoneId]
                let zoneInterval = tz.GetZoneInterval(CurrentInstant)
                orderby zoneInterval.StandardOffset, zoneId
                select new TimeZone(zoneId, zoneInterval.StandardOffset, zoneInterval.WallOffset, tz);

            return list.ToArray();
        }

        public ITimeZone GetTimeZone(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
            {
                return GetSystemTimeZone();
            }

            var dateTimeZone = GetDateTimeZone(timeZoneId);

            return CreateTimeZone(dateTimeZone);
        }

        public ITimeZone GetSystemTimeZone()
        {
            var timezone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            if (TzdbDateTimeZoneSource.Default.CanonicalIdMap.ContainsKey(timezone.Id))
            {
                var canonicalTimeZoneId = TzdbDateTimeZoneSource.Default.CanonicalIdMap[timezone.Id];
                timezone = GetDateTimeZone(canonicalTimeZoneId);
            }
            return CreateTimeZone(timezone);
        }

        public DateTimeOffset ConvertToTimeZone(DateTimeOffset dateTimeOffSet, ITimeZone timeZone)
        {
            var offsetDateTime = OffsetDateTime.FromDateTimeOffset(dateTimeOffSet);
            return offsetDateTime.InZone(((TimeZone)timeZone).DateTimeZone).ToDateTimeOffset();
        }

        internal static DateTimeZone GetDateTimeZone(string timeZone)
        {
            if (!string.IsNullOrEmpty(timeZone) && IsValidTimeZone(DateTimeZoneProviders.Tzdb, timeZone))
            {
                return DateTimeZoneProviders.Tzdb[timeZone];
            }

            return DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        private ITimeZone CreateTimeZone(DateTimeZone dateTimeZone)
        {
            if (dateTimeZone == null)
            {
                throw new ArgumentException(nameof(DateTimeZone));
            }

            var zoneInterval = dateTimeZone.GetZoneInterval(CurrentInstant);

            return new TimeZone(dateTimeZone.Id, zoneInterval.StandardOffset, zoneInterval.WallOffset, dateTimeZone);
        }

        private static bool IsValidTimeZone(IDateTimeZoneProvider provider, string timeZoneId)
        {
            return provider.GetZoneOrNull(timeZoneId) != null;
        }
    }
}
