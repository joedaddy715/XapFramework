namespace Xap.Infrastructure.Logging {
    public enum LoggerLevel {
        /// <summary>Log debug messages.</summary>
        Debug = 1,

        /// <summary>Log informational messages.</summary>
        Info = 2,

        /// <summary>Log success messages.</summary>
        Success = 4,

        /// <summary>Log warning messages.</summary>
        Warning = 8,

        /// <summary>Log error messages.</summary>
        Error = 16,

        /// <summary>Log fatal errors.</summary>
        Fatal = 32,

        /// <summary>Log all messages.</summary>
        All = 0xFFFF,
    }
}
