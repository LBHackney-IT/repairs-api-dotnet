using System;
using Microsoft.Extensions.DependencyInjection;

namespace RepairsApi.V2.UseCase
{
    public interface IActivatorWrapper<T>
    {
        T CreateInstance<TConcrete>() where TConcrete : T;
    }

    public class ActivatorWrapper<T> : IActivatorWrapper<T>
    {
        private readonly IServiceProvider _serviceProvider;

        public ActivatorWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateInstance<TConcrete>() where TConcrete : T
        {
            return ActivatorUtilities.CreateInstance<TConcrete>(_serviceProvider);
        }
    }
}
