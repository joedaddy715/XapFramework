namespace Xap.Logging.Factory.Interfaces {
    public interface IXapLoggingContext
    {
        bool AppendToLog { get; set; }
        uint LoggingLevel { get; set; }
        string ProviderType { get; set; }
        string LogFileLocation { get; set; }
        bool DebugOn { get; set; }
        bool VerboseOn { get; set; }
    }
}
