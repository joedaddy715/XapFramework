using System;
using System.Configuration;
using Xap.Infrastructure.Exceptions;

namespace Xap.Infrastructure.Environment {
    public sealed class XapEnvironment {

        private static readonly XapEnvironment instance = new XapEnvironment();

        static XapEnvironment() { }

        private XapEnvironment() { }

        public static XapEnvironment Instance {
            get { return instance; }
        }

        public string EnvironmentName {
            get {
                try {
                    return ConfigurationManager.AppSettings["environment"];
                } catch {
                    return string.Empty;
                }
            }
        }

        public bool IsWebEnvironment {
            get {
                object env = System.Web.HttpContext.Current;
                if (env == null) {
                    return false;
                }
                return true;
            }
        }

        public string MapFolderPath(string pathName) {
            try {
                if (IsWebEnvironment) {
                    return System.Web.HttpContext.Current.Server.MapPath(pathName);
                }
                return pathName;
            } catch (Exception ex) {
                throw new XapException($"Error executing Server.MapPath for {pathName}",ex);
            }
        }

        private string _configFile = string.Empty;
        public string ConfigurationFile {
            get {
                try {
                    if (string.IsNullOrEmpty(_configPath)) {
                        _configPath = ConfigurationManager.AppSettings["configPath"];
                    }

                    if (string.IsNullOrEmpty(_configFile)) {
                        _configFile = $@"{_configPath}{ConfigurationManager.AppSettings["configFile"]}";
                    }
                    return _configFile;
                } catch {
                    throw;
                }
            }
        }

        private string _configPath = string.Empty;
        public string ConfigurationPath {
            get {
                try {
                    if (string.IsNullOrEmpty(_configPath)) {
                        _configPath = ConfigurationManager.AppSettings["configPath"];
                    }
                    return _configPath;
                } catch {
                    throw;
                }
            }
        }

        private string _dependancyPath = string.Empty;
        public string DependancyPath {
            get {
                try {
                    if (string.IsNullOrEmpty(_dependancyPath)) {
                        _dependancyPath = ConfigurationManager.AppSettings["dependancyPath"];
                    }
                    return _dependancyPath;
                } catch {
                    throw;
                }
            }
        }

        public string GetAppConfigValue(string keyName) {
            try {
                return ConfigurationManager.AppSettings[keyName];
            } catch (Exception ex) {
                throw new XapException($"Error looking up config key {keyName}",ex);
            }
        }
    }
}
