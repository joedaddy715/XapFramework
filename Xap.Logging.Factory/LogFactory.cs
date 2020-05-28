using System;
using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Exceptions;
using Xap.Logging.Factory.Enums;
using Xap.Logging.Factory.Interfaces;

namespace Xap.Logging.Factory {
    public class LogFactory {
        #region "Constructors"

        private static readonly LogFactory instance = new LogFactory();

        static LogFactory() { }

        private LogFactory() { }

        public static LogFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "Provider Loader"
        public IXapLoggingProvider LoadDefaultLoggingProvider() {
            IXapLoggingContext loggingContext;
            try {
                loggingContext = GetLoggingContext($"{XapEnvironment.Instance.EnvironmentName}.logging");

                return AssemblyManager.Instance.CreateInstance<IXapLoggingProvider>(loggingContext.ProviderType);
            } catch (Exception ex) {
                throw new XapException($"Error loading logging provider", ex);
            }
        }

        public IXapLoggingProvider LoadLoggingProvider(IXapLoggingContext loggingContext) {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapLoggingProvider>(loggingContext.ProviderType);
            } catch (Exception ex) {
                throw new XapException($"Error loading logging provider {loggingContext.ProviderType}", ex);
            }
        }

        public IXapLoggingProvider LoadLoggingProvider(string configurationKey) {
            IXapLoggingContext loggingContext;
            try {
                loggingContext = GetLoggingContext(configurationKey);
                return AssemblyManager.Instance.CreateInstance<IXapLoggingProvider>(loggingContext.ProviderType);
            } catch (Exception ex) {
                throw new XapException($"Error loading logging provider", ex);
            }
        }
        #endregion

        #region "private methods"
        private IXapLoggingContext GetLoggingContext(string configurationKey) {
            IXapLoggingContext loggingContext = LoggingContext.Create();

            if (XapConfig.Instance.ContainsKey($"{configurationKey}", "debugOn")) {
                loggingContext.DebugOn = XapConfig.Instance.GetValue<bool>($"{configurationKey}", "debugOn");
            }

            if (XapConfig.Instance.ContainsKey($"{configurationKey}", "verboseOn")) {
                loggingContext.VerboseOn = XapConfig.Instance.GetValue<bool>($"{configurationKey}", "verboseOn");
            }

            if (XapConfig.Instance.ContainsKey($"{configurationKey}", "logFile")) {
                loggingContext.LogFileLocation = XapConfig.Instance.GetValue<string>($"{configurationKey}", "logFile");
            }

            if (XapConfig.Instance.ContainsKey($"{configurationKey}", "providerType")) {
                loggingContext.ProviderType = XapConfig.Instance.GetValue<string>($"{configurationKey}", "providerType");
            }

            if (XapConfig.Instance.ContainsKey($"{configurationKey}", "loggingLevel")) {
                string level = XapConfig.Instance.GetValue<string>($"{configurationKey}", "loggingLevel");
                switch (level) {
                    case "debug":
                        loggingContext.LoggingLevel = (uint)LoggerLevel.Debug;
                        break;
                    case "info":
                        loggingContext.LoggingLevel = (uint)LoggerLevel.Info;
                        break;
                    case "success":
                        loggingContext.LoggingLevel = (uint)LoggerLevel.Success;
                        break;
                    case "warning":
                        loggingContext.LoggingLevel = (uint)LoggerLevel.Warning;
                        break;
                    case "error":
                        loggingContext.LoggingLevel = (uint)LoggerLevel.Error;
                        break;
                    case "fatal":
                        loggingContext.LoggingLevel = (uint)LoggerLevel.Fatal;
                        break;
                    default:
                        loggingContext.LoggingLevel = (uint)LoggerLevel.All;
                        break;
                }
            }
            return loggingContext;
        }
        #endregion
    }
}
