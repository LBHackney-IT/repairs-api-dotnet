using Newtonsoft.Json;

namespace RepairsApi.Tests.Helpers
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T a)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(a));
        }

        /// <summary>
        /// For Loading Nvaigation Properties in tests
        /// </summary>
        /// <param name="o"></param>
        public static void Load(this object o)
        {
            o.ToString();
        }
    }
}
