namespace FreeDB.Web.Helpers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal static class StreamExtensions
    {
        internal static Task<object> ReadAsJson(this Stream stream, Type instanceType, JsonSerializer serializer)
        {
            if (stream == null)
                return null;

            return Task<object>.Factory.StartNew(
                () =>
                    {
                        using (var reader = new JsonTextReader(new StreamReader(stream)))
                        {
                            var obj = serializer.Deserialize(reader);
                            return obj.GetType().IsSubclassOf(instanceType)
                                       ? obj
                                       : serializer.Deserialize(reader, instanceType);
                        }
                    }
                );
        }

        internal static Task WriteAsJson(this Stream stream, object instance, JsonSerializer serializer)
        {
            if (stream == null)
                return null;

            return Task.Factory.StartNew(
                () =>
                    {
                        using (var writer = new JsonTextWriter(new StreamWriter(stream)))
                        {
                            serializer.Serialize(writer, instance);
                            writer.Flush();
                        }
                    }
                );
        }
    }
}