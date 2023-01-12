using Newtonsoft.Json;

namespace ShoppingCart.Infrastructure
{
   static public class SessionExtentions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            var DataInsession = sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
            return DataInsession;
        }
    }
}
