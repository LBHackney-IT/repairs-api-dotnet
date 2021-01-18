using System;
using System.Collections;
using System.Collections.Generic;

namespace RepairsApi.Tests.Helpers.StubGeneration
{

    public class Generator<T>
    {
        private readonly Dictionary<Type, IValueGenerator> _generators;
        private readonly Stack<Type> _typeStack;

        public Generator()
            : this(new Dictionary<Type, IValueGenerator>())
        {
        }

        public Generator(Dictionary<Type, IValueGenerator> generators)
        {
            _typeStack = new Stack<Type>();
            _generators = generators;

            _generators.TryAdd(typeof(string), new SimpleValueGenerator<string>(() => string.Empty));
        }

        public Generator<T> AddGenerator<TValue>(AbstractValueGenerator<TValue> generator)
        {
            Type valueType = typeof(TValue);
            if (_generators.ContainsKey(valueType)) _generators.Remove(valueType);

            _generators.Add(valueType, generator);
            return this;
        }

        public T Generate()
        {
            var type = (T) Run(typeof(T));
            _typeStack.Clear();
            return type;
        }

        public List<T> GenerateList(int count)
        {
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                list.Add(Generate());
            }

            return list;
        }

        private object Run(Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type) ?? type;

            if (_typeStack.Contains(actualType)) return null;

            _typeStack.Push(actualType);

            IValueGenerator gen;
            if (_generators.TryGetValue(actualType, out gen))
            {
                _typeStack.Pop();
                return gen.Create();
            }

            if (actualType.IsValueType)
            {
                _typeStack.Pop();
                return null;
            }

            if (actualType.IsGenericType)
            {
                var typeParams = actualType.GetGenericArguments();
                if (actualType.IsAssignableFrom(typeof(List<>).MakeGenericType(typeParams)))
                {
                    _typeStack.Pop();
                    return CreateList(typeParams[0]);
                }
            }

            var result = Activator.CreateInstance(actualType);

            foreach (var item in actualType.GetProperties())
            {
                item.SetValue(result, Run(item.PropertyType));
            }

            _typeStack.Pop();
            return result;
        }

        private object CreateList(Type type)
        {
            Type listType = typeof(List<>).MakeGenericType(type);
            var list = (IList) Activator.CreateInstance(listType);

            int limit = new Random().Next(9) + 1;
            for (int i = 0; i < limit; i++)
            {
                list.Add(Run(type));
            }

            return list;
        }
    }

    public interface IValueGenerator
    {
        object Create();
    }

    public abstract class AbstractValueGenerator<T> : IValueGenerator
    {
        public object Create()
        {
            return GenerateValue();
        }

        public abstract T GenerateValue();
    }

    public class SimpleValueGenerator<T> : AbstractValueGenerator<T>
    {
        private readonly Func<T> _gen;

        public SimpleValueGenerator(T value)
            : this(() => value)
        { }

        public SimpleValueGenerator(Func<T> generatorFunction)
        {
            _gen = generatorFunction;
        }

        public override T GenerateValue()
        {
            return _gen.Invoke();
        }
    }
}
