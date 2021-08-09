using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests
{
    public sealed class XUnitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper output;

        public XUnitLoggerProvider(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void Dispose()
        {
            // Method intentionally left empty.
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(output, categoryName);
        }

        public ILogger<TCategoryName> CreateLogger<TCategoryName>()
        {
            return new XUnitLogger<TCategoryName>(output);
        }
    }

    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper output;
        private readonly string category;

        public XUnitLogger(ITestOutputHelper output, string category)
        {
            this.output = output;
            this.category = category;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null) return;
            var line = $"{category} {logLevel}: {message}";
            if (exception != null) line = $"{line}{Environment.NewLine}{exception.ToString()}";
            output.WriteLine(line);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class XUnitLogger<T> : XUnitLogger, ILogger<T>
    {
        public XUnitLogger(ITestOutputHelper output) : base(output, typeof(T).Name)
        {
        }
    }
}