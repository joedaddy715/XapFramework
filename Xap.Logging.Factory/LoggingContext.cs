using Xap.Logging.Factory.Enums;
using Xap.Logging.Factory.Interfaces;
using Xap.Logging.Factory.Providers;

namespace Xap.Logging.Factory {
    public class LoggingContext : IXapLoggingContext {
        private LoggingContext() { }
        public static IXapLoggingContext Create() {
            return new LoggingContext();
        }

        private bool _appendToLog = false;
        bool IXapLoggingContext.AppendToLog {
            get => _appendToLog;
            set => _appendToLog = value;
        }

        private uint _loggingLevel = (uint)LoggerLevel.All;
        uint IXapLoggingContext.LoggingLevel {
            get => _loggingLevel;
            set => _loggingLevel = value;
        }

        private string _providerType = LoggingProviderTypes.TextFile;
        string IXapLoggingContext.ProviderType {
            get => _providerType;
            set => _providerType = value;
        }

        private string _logFileLocation = string.Empty;
        string IXapLoggingContext.LogFileLocation {
            get => _logFileLocation;
            set => _logFileLocation = value;
        }

        private bool _debugOn = false;
        bool IXapLoggingContext.DebugOn {
            get => _debugOn;
            set => _debugOn = value;
        }

        private bool _verboseOn = false;
        bool IXapLoggingContext.VerboseOn {
            get => _verboseOn;
            set => _verboseOn = value;
        }
    }
}
