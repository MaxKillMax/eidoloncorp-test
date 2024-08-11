using Newtonsoft.Json;

namespace Test.Parsers
{
    public static class Parser
    {
        public static string Serialize<T>(T @object) => JsonConvert.SerializeObject(@object);

        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);

        public static bool TryDeserialize<T>(string json, out T t)
        {
            try
            {
                t = Deserialize<T>(json);
                return true;
            }
            catch
            {
                t = default;
                return false;
            }
        }
    }
}
