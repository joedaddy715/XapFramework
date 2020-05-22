namespace Xap.Infrastructure.Logging {
    public enum LoggerState {
        /// <summary>The logger is stopped.</summary>
        Stopped = 0,

        /// <summary>The logger has been started.</summary>
        Running,

        /// <summary>The logger is paused.</summary>
        Paused,
    }
}
