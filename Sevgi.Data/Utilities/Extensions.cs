using Newtonsoft.Json;

namespace Sevgi.Data.Utilities
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

        public static string GenerateEmailForFirebase(this string seed)
        {
            return "fuser_" + seed.ToLower() + "@firebase.com";
        }
        public static string GenerateUsernameForFirebase(this string seed, string second)
        {
            return "fuser_" + seed + "_" + second + "_" + (seed + second).Length;
        }
        public static string GenerateEmailForInternal(this string phoneNumber)
        {
            return "intuser_" + phoneNumber.ToLower() + phoneNumber.Length + "@internal.com";
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
