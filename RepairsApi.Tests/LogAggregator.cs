using System;
using System.Collections.Generic;

namespace RepairsApi.Tests
{
    public class LogAggregator
    {
        public List<LogRecord> Logs { get; } = new List<LogRecord>();

        public string All => ToString();

        internal void Log<T>(string message)
        {
            Logs.Add(new LogRecord(typeof(T), message));
        }

        public override string ToString()
        {
            return string.Join('\n', Logs);
        }
    }

    public class LogRecord
    {
        private Type _type;
        private string _message;

        public LogRecord(Type type, string message)
        {
            _type = type;
            _message = message;
        }

        public override string ToString()
        {
            return $"{_type.Name}: {_message}";
        }
    }
}
