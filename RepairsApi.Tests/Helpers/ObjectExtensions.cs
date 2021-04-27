using Newtonsoft.Json;

namespace RepairsApi.Tests.Helpers
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T a)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(a));
        }
    }
}
