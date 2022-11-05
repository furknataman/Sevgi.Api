using Newtonsoft.Json;

namespace ProjectBase.Data.Utilities
{
    public static class Extensions
    {
        public static string GeneratePassword(this string seed)
        {
            return seed[0].ToString().ToUpper() + seed.ToLower() + seed.Length;
        }

        public static bool VerifyPassword(string seed, string password)
        {
            return seed.GeneratePassword() == password;
        }

    }

    public static class JsonExtensions
    {
        public static T? FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json);

        public static string ToJson<T>(this T o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public static string ToJson<T>(this T o, bool indent)
        {
            return JsonConvert.SerializeObject(o, indent ? Formatting.Indented : Formatting.None);
        }

        public static T? FromAnonymousJson<T>(this string json, T anonymousType) => JsonConvert.DeserializeAnonymousType(json, anonymousType);
    }
}
