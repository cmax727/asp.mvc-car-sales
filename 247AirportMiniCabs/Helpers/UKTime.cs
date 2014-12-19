using System;

namespace AlinTuriCab.Helpers
{
    public static class UKTime
    {
        public static DateTime Now
        {
            get
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
            }
        }
    }
}