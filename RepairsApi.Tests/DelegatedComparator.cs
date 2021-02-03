using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RepairsApi.Tests
{
    internal class DelegatedComparator<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T, T, bool> _p;

        public DelegatedComparator(Func<T, T, bool> p)
        {
            _p = p;
        }

        public bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            return _p.Invoke(x, y);
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }
    }
}
