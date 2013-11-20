namespace FreeDB.Core.Common
{
    using System;
    using System.Globalization;
    using Model;

    public static class StringExtensions
    {
        public static string TruncateAndAddEllipsis(this string value, int maxLength)
        {
            return value.Length <= maxLength
                       ? value
                       : value.Substring(0, maxLength - 3) + "...";
        }

        public static string ToYearRange(Tuple<Disc, Disc> firstAndLast)
        {
            return String.Format("{0} - {1}",
                                 firstAndLast.Item1 != null
                                     ? firstAndLast.Item1.Released.GetValueOrDefault()
                                                   .Year.ToString(CultureInfo.InvariantCulture)
                                     : "?",
                                 firstAndLast.Item2 != null
                                     ? firstAndLast.Item2.Released.GetValueOrDefault()
                                                   .Year.ToString(CultureInfo.InvariantCulture)
                                     : "?");
        }
    }
}
