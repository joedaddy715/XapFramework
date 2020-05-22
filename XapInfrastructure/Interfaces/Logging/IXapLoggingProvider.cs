using Xap.Infrastructure.Logging;

namespace Xap.Infrastructure.Interfaces.Logging {
    public interface IXapLoggingProvider {
        bool Start(bool bAppend);
        bool Start(bool bAppend, uint logLevels, string criteria);
        bool Start(uint logLevels, string criteria);
        bool Stop();
        bool Resume();
        bool Pause();
        bool Fatal(string msg);
        bool FatalVerbose(string msg);
        bool Info(string msg);
        bool InfoVerbose(string msg);
        bool Error(string msg);
        bool ErrorVerbose(string msg);
        bool Success(string msg);
        bool SuccessVerbose(string msg);
        bool Warning(string msg);
        bool WarningVerbose(string msg);
        bool Debug(string msg);
        bool DebugVerbose(string msg);
        bool Write(string msg);
        bool DebugOn { get; set; }
        bool VerboseOn { get; set; }
        uint GetMessageCount(uint levelMask);
        uint FatalMsgs { get; set; }
        uint InfoMsgs { get; set; }
        uint SuccessMsgs { get; set; }
        uint WarningMsgs { get; set; }
        uint DebugMsgs { get; set; }
        uint ErrorMsgs { get; set; }
        LoggerState State { get; set; }
        LoggerLevel Level { get; set; }
        uint Levels { get; set; }
    }
}
