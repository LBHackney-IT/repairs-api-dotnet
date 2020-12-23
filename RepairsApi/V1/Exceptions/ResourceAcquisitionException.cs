using System;

namespace RepairsApi.V1.Exceptions
{
    public abstract class ResourceAcquisitionException : Exception
    {
        public abstract int StatusCode { get; }
    }
}
