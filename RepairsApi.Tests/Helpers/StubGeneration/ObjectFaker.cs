using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepairsApi.Tests.Helpers.StubGeneration
{

    public class Generator<T>
    {
        private readonly Dictionary<Type, IValueGenerator> _generators;
        private readonly Dictionary<Type, int> _listLengths;
        private readonly Dictionary<MemberInfo, IValueGenerator> _propertyGenerators;
        private readonly Stack<Type> _typeStack;

        public Generator()
            : this(new Dictionary<Type, IValueGenerator>())
        {
        }

        public Generator(Dictionary<Type, IValueGenerator> generators)
        {
            _propertyGenerators = new Dictionary<MemberInfo, IValueGenerator>();
            _typeStack = new Stack<Type>();
            _generators = generators;
            _listLengths = new Dictionary<Type, int>();

            _generators.TryAdd(typeof(string), new SimpleValueGenerator<string>(() => string.Empty));
        }

        public Generator<T> AddGenerator<TValue>(AbstractValueGenerator<TValue> generator)
        {
            Type valueType = typeof(TValue);
            if (_generators.ContainsKey(valueType)) _generators.Remove(valueType);

            _generators.Add(valueType, generator);
            return this;
        }

        public Generator<T> AddGenerator<TModel, TProp>(AbstractValueGenerator<TProp> generator, params Expression<Func<TModel, TProp>>[] accessors)
        {
            foreach (var accessor in accessors)
            {
                if (accessor.Body.NodeType != ExpressionType.MemberAccess) throw new Exception("accessor must be simple property accessor eg obj.prop");
                MemberExpression casted = accessor.Body as MemberExpression;

                if (_propertyGenerators.ContainsKey(casted.Member)) _propertyGenerators.Remove(casted.Member);
                _propertyGenerators.TryAdd(casted.Member, generator);
            }

            return this;
        }

        public Generator<T> SetListLength<TValue>(int length)
        {
            Type valueType = typeof(TValue);
            if (_listLengths.ContainsKey(valueType)) _listLengths.Remove(valueType);

            _listLengths.Add(valueType, length);
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

            if (_generators.TryGetValue(actualType, out IValueGenerator gen))
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

            if (actualType.IsArray)
            {
                var innerType = actualType.GetElementType();
                _typeStack.Pop();
                return CreateArray(innerType);
            }

            var result = Activator.CreateInstance(actualType);

            foreach (var item in actualType.GetProperties())
            {
                var key = _propertyGenerators.Keys.SingleOrDefault(k => item.DeclaringType.FullName == k.DeclaringType.FullName && item.Name == k.Name);

                if (key != null)
                {
                    item.SetValue(result, _propertyGenerators[key].Create());
                }
                else
                {
                    item.SetValue(result, Run(item.PropertyType));
                }

            }

            _typeStack.Pop();
            return result;
        }

        private object CreateList(Type type)
        {
            Type listType = typeof(List<>).MakeGenericType(type);
            var list = (IList) Activator.CreateInstance(listType);

            var limit = new Random().Next(9) + 1;

            if (_listLengths.TryGetValue(type, out var listLength))
            {
                limit = listLength;
            }

            for (int i = 0; i < limit; i++)
            {
                list.Add(Run(type));
            }

            return list;
        }

        private object CreateArray(Type type)
        {
            var limit = new Random().Next(9) + 1;
            var list = Array.CreateInstance(type, limit);

            if (_listLengths.TryGetValue(type, out var listLength))
            {
                limit = listLength;
            }

            for (int i = 0; i < limit; i++)
            {
                list.SetValue(Run(type), i);
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
