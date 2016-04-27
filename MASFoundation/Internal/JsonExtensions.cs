using Windows.Data.Json;

namespace MASFoundation.Internal
{
    static internal class JsonExtensions
    {
        public static string GetStringOrNull(this JsonObject obj, string key)
        {
            IJsonValue value;
            if (obj.TryGetValue(key, out value))
            {
                return value.GetString();
            }

            return null;
        }
    }
}
