using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Obj.Genarator
{

    class Generator<T>
    {
        private readonly Dictionary<Type, IGenerator> _generators;
        private readonly Stack<Type> _typeStack;

        public Generator()
            : this(new Dictionary<Type, IGenerator>())
        {
        }

        public Generator(Dictionary<Type, IGenerator> generators)
        {
            _typeStack = new Stack<Type>();
            _generators = generators;

            _generators.TryAdd(typeof(string), new Generator(() => string.Empty));
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

            IGenerator gen;
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

    public interface IGenerator
    {
        object Create();
    }

    public class Generator : IGenerator
    {
        private readonly Func<object> _gen;

        public Generator(Func<object> gen)
        {
            _gen = gen;
        }

        public object Create()
        {
            return _gen.Invoke();
        }
    }

    public class RandomStringGenerator : IGenerator
    {
        private readonly int _length;
        private static readonly Random _random = new Random();

        public RandomStringGenerator(int length)
        {
            _length = length;
        }

        public object Create()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, _length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }

    public class RandomDoubleGenerator : IGenerator
    {
        private readonly double _min;
        private readonly double _max;
        private static readonly Random _random = new Random();

        public RandomDoubleGenerator(double min = 0, double max = 10)
        {
            _min = min;
            _max = max;
        }

        public object Create()
        {
            return (_random.NextDouble() * (_max - _min)) + _min;
        }
    }

    public class RandomBoolGenerator : IGenerator
    {
        private static readonly Random _random = new Random();

        public RandomBoolGenerator()
        {
        }

        public object Create()
        {
            return _random.NextDouble() >= 0.5;
        }
    }
}
