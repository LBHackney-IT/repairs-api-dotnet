using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all implementors of the serviceType as transient
        /// </summary>
        /// <param name="services">collection services are added to</param>
        /// <param name="assemblyMarkerType">Type used to determine the assembly to scan for implementations</param>
        /// <param name="serviceType">service type (can be an open generic)</param>
        public static void AddTransients(this IServiceCollection services, Type assemblyMarkerType, Type serviceType)
        {
            var assembly = assemblyMarkerType.Assembly;

            var handlers = assembly.GetTypes().Where(t => t.IsClass && t.GetInterfaces().Any(i => i == serviceType || i.MatchesOpenGeneric(serviceType)));

            foreach (var handler in handlers)
            {
                foreach (var item in handler.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == serviceType))
                {
                    services.AddTransient(item, handler);
                }
            }
        }

        private static bool MatchesOpenGeneric(this Type type, Type openGeneric)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric;
        }
    }
}
