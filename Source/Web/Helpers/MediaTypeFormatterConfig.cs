namespace FreeDB.Web.Helpers
{
    using System.Linq;
    using System.Net.Http.Formatting;

    public class MediaTypeFormatterConfig
    {
        public static void RegisterJsonNetMediaTypeFormatter(MediaTypeFormatterCollection formatters)
        {
            formatters.Remove(formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault());
            formatters.Add(new JsonNetMediaTypeFormatter());
        }
    }
}