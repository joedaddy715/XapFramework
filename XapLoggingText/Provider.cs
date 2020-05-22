using System;
using System.IO;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Interfaces.Logging;
using Xap.Infrastructure.Logging;

namespace Xap.Logging.Text {
    public class Provider : IXapLoggingProvider {
        private string _logFilename;
        private bool _bAppend = true;
        private StreamWriter _logFile = null;


        public Provider() {
        }

        #region IXapLoggingProvider Members

        private bool _debugOn = false;
        bool IXapLoggingProvider.DebugOn {
            get { return _debugOn; }
            set { _debugOn = value; }
        }

        private bool _verboseOn = false;
        bool IXapLoggingProvider.VerboseOn {
            get { return _verboseOn; }
            set { _verboseOn = value; }
        }

        uint IXapLoggingProvider.GetMessageCount(uint levelMask) {
            try {
                uint uMessages = 0;
                if ((levelMask & ((uint)LoggerLevel.Debug)) != 0)
                    uMessages += _debugMsgs;
                if ((levelMask & ((uint)LoggerLevel.Info)) != 0)
                    uMessages += _infoMsgs;
                if ((levelMask & ((uint)LoggerLevel.Success)) != 0)
                    uMessages += _successMsgs;
                if ((levelMask & ((uint)LoggerLevel.Warning)) != 0)
                    uMessages += _warningMsgs;
                if ((levelMask & ((uint)LoggerLevel.Error)) != 0)
                    uMessages += _errorMsgs;
                if ((levelMask & ((uint)LoggerLevel.Fatal)) != 0)
                    uMessages += _fatalMsgs;
                return uMessages;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        uint _fatalMsgs;
        uint IXapLoggingProvider.FatalMsgs {
            get { return _fatalMsgs; }
            set { _fatalMsgs = value; }
        }

        private uint _infoMsgs;
        uint IXapLoggingProvider.InfoMsgs {
            get { return _infoMsgs; }
            set { _infoMsgs = value; }
        }

        private uint _successMsgs;
        uint IXapLoggingProvider.SuccessMsgs {
            get { return _successMsgs; }
            set { _successMsgs = value; }
        }

        private uint _warningMsgs;
        uint IXapLoggingProvider.WarningMsgs {
            get { return _warningMsgs; }
            set { _warningMsgs = value; }
        }

        private uint _debugMsgs;
        uint IXapLoggingProvider.DebugMsgs {
            get { return _debugMsgs; }
            set { _debugMsgs = value; }
        }

        private uint _errorMsgs;
        uint IXapLoggingProvider.ErrorMsgs {
            get { return _errorMsgs; }
            set { _errorMsgs = value; }
        }

        private LoggerState _loggerState;
        LoggerState IXapLoggingProvider.State {
            get { return _loggerState; }
            set { _loggerState = value; }
        }

        private LoggerLevel _loggerLevel;
        LoggerLevel IXapLoggingProvider.Level {
            get { return _loggerLevel; }
            set { _loggerLevel = value; }
        }

        private uint _levels;
        uint IXapLoggingProvider.Levels {
            get { return _levels; }
            set { _levels = value; }
        }

        bool IXapLoggingProvider.Start(bool bAppend) {
            if (XapConfig.Instance.ContainsKey($"{XapEnvironment.Instance.EnvironmentName}.logging", "debugOn")) {
                _debugOn = XapConfig.Instance.GetValue<bool>($"{XapEnvironment.Instance.EnvironmentName}.logging", "debugOn");
            }
            if (XapConfig.Instance.ContainsKey($"{XapEnvironment.Instance.EnvironmentName}.logging", "verboseOn")) {
                _verboseOn = XapConfig.Instance.GetValue<bool>($"{XapEnvironment.Instance.EnvironmentName}.logging", "verboseOn");
            }
            return StartLog(bAppend, (uint)LoggerLevel.All, string.Empty);
        }

        bool IXapLoggingProvider.Start(uint logLevels, string fileName) {
            if (XapConfig.Instance.ContainsKey($"{XapEnvironment.Instance.EnvironmentName}.logging", "debugOn")) {
                _debugOn = XapConfig.Instance.GetValue<bool>($"{XapEnvironment.Instance.EnvironmentName}.logging", "debugOn");
            }
            if (XapConfig.Instance.ContainsKey($"{XapEnvironment.Instance.EnvironmentName}.logging", "verboseOn")) {
                _verboseOn = XapConfig.Instance.GetValue<bool>($"{XapEnvironment.Instance.EnvironmentName}.logging", "verboseOn");
            }
            return StartLog(true, logLevels, fileName);
        }

        bool IXapLoggingProvider.Start(bool bAppend, uint logLevels, string fileName) {
            if (XapConfig.Instance.ContainsKey($"{XapEnvironment.Instance.EnvironmentName}.logging", "debugOn")) {
                _debugOn = XapConfig.Instance.GetValue<bool>($"{XapEnvironment.Instance.EnvironmentName}.logging", "debugOn");
            }
            if (XapConfig.Instance.ContainsKey($"{XapEnvironment.Instance.EnvironmentName}.logging", "verboseOn")) {
                _verboseOn = XapConfig.Instance.GetValue<bool>($"{XapEnvironment.Instance.EnvironmentName}.logging", "verboseOn");
            }
            return StartLog(bAppend, logLevels, fileName);
        }

        bool IXapLoggingProvider.Stop() {
            lock (this) {
                // Fail if logging hasn't been started
                if (_loggerState != LoggerState.Running)
                    return false;

                // Stop logging
                try {
                    _logFile.Close();
                    _logFile = null;
                } catch {
                    return false;
                }
                _loggerState = LoggerState.Stopped;
                return true;
            }
        }

        bool IXapLoggingProvider.Resume() {
            lock (this) {
                // Fail if logging hasn't been paused
                if (_loggerState != LoggerState.Paused)
                    return false;

                // Resume logging
                _loggerState = LoggerState.Running;
                return true;
            }
        }

        bool IXapLoggingProvider.Pause() {
            lock (this) {
                // Fail if logging hasn't been started
                if (_loggerState != LoggerState.Running)
                    return false;

                // Pause the logger
                _loggerState = LoggerState.Paused;
                return true;
            }
        }

        bool IXapLoggingProvider.Fatal(string msg) {
            if (_loggerState != LoggerState.Running) {
                return false;
            } else if (_loggerState == LoggerState.Paused) {
                return true;
            }

            _fatalMsgs++;
            if (_debugOn) {
                return WriteLogMsg(LoggerLevel.Fatal, msg);
            }
            return true;
        }

        bool IXapLoggingProvider.FatalVerbose(string msg) {
            if (_debugOn && _verboseOn) {
                return ((IXapLoggingProvider)this).Fatal(msg);
            }
            return true;
        }

        bool IXapLoggingProvider.Info(string msg) {
            if (_loggerState != LoggerState.Running) {
                return false;
            } else if (_loggerState == LoggerState.Paused) {
                return true;
            }
            _infoMsgs++;

            if (_debugOn) {
                return WriteLogMsg(LoggerLevel.Info, msg);
            }
            return true;
        }

        bool IXapLoggingProvider.InfoVerbose(string msg) {
            if (_debugOn && _verboseOn) {
                return ((IXapLoggingProvider)this).Info(msg);
            }
            return true;
        }

        bool IXapLoggingProvider.Error(string msg) {
            if (_loggerState != LoggerState.Running) {
                return false;
            } else if (_loggerState == LoggerState.Paused) {
                return true;
            }
            _errorMsgs++;

            if (_debugOn) {
                return WriteLogMsg(LoggerLevel.Error, msg);
            }
            return true;
        }

        bool IXapLoggingProvider.ErrorVerbose(string msg) {
            if (_debugOn && _verboseOn) {
                return ((IXapLoggingProvider)this).Error(msg);
            }
            return true;
        }

        bool IXapLoggingProvider.Success(string msg) {
            if (_loggerState != LoggerState.Running) {
                return false;
            } else if (_loggerState == LoggerState.Paused) {
                return true;
            }
            _successMsgs++;

            if (_debugOn) {
                return WriteLogMsg(LoggerLevel.Success, msg);
            }
            return true;
        }

        bool IXapLoggingProvider.SuccessVerbose(string msg) {
            if (_debugOn && _verboseOn) {
                return ((IXapLoggingProvider)this).Success(msg);
            }
            return true;
        }

        bool IXapLoggingProvider.Warning(string msg) {
            if (_loggerState != LoggerState.Running) {
                return false;
            } else if (_loggerState == LoggerState.Paused) {
                return true;
            }
            _warningMsgs++;

            if (_debugOn) {
                return WriteLogMsg(LoggerLevel.Warning, msg);
            }
            return true;
        }

        bool IXapLoggingProvider.WarningVerbose(string msg) {
            if (_debugOn && _verboseOn) {
                return ((IXapLoggingProvider)this).Warning(msg);
            }
            return true;
        }


        bool IXapLoggingProvider.Debug(string msg) {
            if (_loggerState != LoggerState.Running) {
                return false;
            } else if (_loggerState == LoggerState.Paused) {
                return true;
            }
            _debugMsgs++;

            if (_debugOn) {
                return WriteLogMsg(LoggerLevel.Debug, msg);
            }
            return true;
        }

        bool IXapLoggingProvider.DebugVerbose(string msg) {
            if (_debugOn && _verboseOn) {
                return ((IXapLoggingProvider)this).Debug(msg);
            }
            return true;
        }

        bool IXapLoggingProvider.Write(string msg) {
            return WriteMsg(msg);
        }

        #endregion

        private bool WriteMsg
          (string msg) {
            lock (this) {

                // Fail if logger hasn't been started
                if (_loggerState == LoggerState.Stopped)
                    return false;

                // Ignore message logging is paused or it doesn't pass the filter
                if ((_loggerState == LoggerState.Paused))
                    return true;

                // Write log message
                _logFile.WriteLine(msg);
                return true;
            }
        }

        protected virtual bool WriteLogMsg
       (LoggerLevel level,
        string msg) {
            lock (this) {

                // Fail if logger hasn't been started
                if (_loggerState == LoggerState.Stopped)
                    return false;

                // Ignore message logging is paused or it doesn't pass the filter
                if ((_loggerState == LoggerState.Paused) || ((_levels & (uint)level) != (uint)level))
                    return true;

                // Write log message
                DateTime tmNow = DateTime.Now;
                string logMsg = String.Format("{0} {1}  {2}: {3}",
                                               tmNow.ToShortDateString(), tmNow.ToLongTimeString(),
                                               level.ToString().Substring(0, 1), msg);
                _logFile.WriteLine(logMsg);
                return true;
            }
        }

        protected virtual bool StartLog(bool bAppend, uint logLevels, string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                _logFilename = XapEnvironment.Instance.MapFolderPath(XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.logging", "logFile"));
            } else {
                _logFilename = fileName;
            }


            _bAppend = bAppend;
            _levels = logLevels;

            lock (this) {
                // Fail if logging has already been started
                if (_loggerState != LoggerState.Stopped)
                    return false;

                // Fail if the log file isn't specified
                if (String.IsNullOrEmpty(_logFilename))
                    return false;

                // Delete log file if it exists
                if (!_bAppend) {
                    try {
                        File.Delete(_logFilename);
                    } catch (Exception) {
                        return false;
                    }
                }

                // Open file for writing - return on error
                if (!File.Exists(_logFilename)) {
                    try {
                        _logFile = File.CreateText(_logFilename);
                    } catch (Exception) {
                        _logFile = null;
                        return false;
                    }
                } else {
                    try {
                        _logFile = File.AppendText(_logFilename);
                    } catch {
                        _logFile = null;
                        return false;
                    }
                }
                _logFile.AutoFlush = true;

                // Return successfully
                _loggerState = LoggerState.Running;
                return true;
            }
        }
    }
}

