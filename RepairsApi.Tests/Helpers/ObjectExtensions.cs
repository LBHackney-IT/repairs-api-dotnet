using Newtonsoft.Json;
using System.Collections.Generic;

namespace RepairsApi.Tests.Helpers
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T a)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(a));
        }

        public static T[] MakeArray<T>(this T o) => new T[] { o };
        public static List<T> MakeList<T>(this T o) => new List<T>(o.MakeArray());

        /// <summary>
        /// For Loading Navigation Properties in tests
        /// </summary>
        /// <param name="o"></param>
        public static void Load(this object o)
        {
            o.ToString(); // Calling a function on the object will cause navigation proeprties to be loaded
        }
    }
}
