namespace FreeDB.Core.Common
{
    using System;

    public static class TimeSpanExtensions
    {
        public static string GetHoursAndMinutesString(int lengthInSeconds)
        {
            var timeSpan = TimeSpan.FromSeconds(lengthInSeconds);
            var runtime = String.Format("{0}h {1}m", timeSpan.Hours, timeSpan.Minutes);
            return runtime;
        }

        public static string GetMinutesAndSecondsString(int lengthInSeconds)
        {
            var timeSpan = TimeSpan.FromSeconds(lengthInSeconds);
            var runtime = String.Format("{0}m {1}s", timeSpan.Minutes, timeSpan.Seconds);
            return runtime;
        }
    }
}
