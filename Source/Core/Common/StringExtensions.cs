namespace FreeDB.Core.Common
{
    public static class StringExtensions
    {
        public static string TruncateAndAddEllipsis(this string value, int maxLength)
        {
            return value.Length <= maxLength
                       ? value
                       : value.Substring(0, maxLength - 3) + "...";
        }
    }
}
