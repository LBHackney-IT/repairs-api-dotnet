using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi
{
    public class LazyWrapper<T> : Lazy<T>
    {
        public LazyWrapper(IServiceProvider serviceProvider)
            : base(() => serviceProvider.GetRequiredService<T>())
        {

        }
    }
}
