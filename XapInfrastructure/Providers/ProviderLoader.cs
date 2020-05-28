using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Interfaces.Configuration;
using Xap.Infrastructure.Interfaces.Security;
using Xap.Infrastructure.Interfaces.Validation;

namespace Xap.Infrastructure.Providers {
    public class ProviderLoader {
        private static readonly ProviderLoader instance = new ProviderLoader();
        
        static ProviderLoader() { }

        private ProviderLoader() { }

        public static ProviderLoader Instance {
            get { return instance; }
        }

        public void LoadProviders() {
            try {
                if (XapConfig.Instance.ContainsSection($"{XapEnvironment.Instance.EnvironmentName}.providers")) {
                    foreach (string _interface in XapConfig.Instance.GetKeys($"{XapEnvironment.Instance.EnvironmentName}.providers")) {
                        AssemblyManager.Instance.LoadAssemblies(XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.providers", _interface), _interface);
                    }
                }
            } catch {
                throw;
            }
        }

        public IXapConfigurationProvider LoadConfigurationProvider() {
            try {
                AssemblyManager.Instance.ClearCache();
                AssemblyManager.Instance.LoadAssemblies(XapEnvironment.Instance.GetAppConfigValue("configurationProviderPath"), "IXapConfigurationProvider");
                return AssemblyManager.Instance.CreateInstance<IXapConfigurationProvider>(XapEnvironment.Instance.GetAppConfigValue("configurationProvider"));
            } catch {
                throw;
            }
        }

        public IXapConfigurationProvider LoadConfigurationProvider(string providerType) {
            try {
                AssemblyManager.Instance.ClearCache();
                AssemblyManager.Instance.LoadAssemblies(XapEnvironment.Instance.GetAppConfigValue("configurationProviderPath"), "IXapConfigurationProvider");
                return AssemblyManager.Instance.CreateInstance<IXapConfigurationProvider>(XapEnvironment.Instance.GetAppConfigValue(providerType));
            } catch {
                throw;
            }
        }

        public IXapSecurityProvider LoadSecurityProvider() {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapSecurityProvider>(XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.security", "provider"));
            } catch {
                throw;
            }
        }

        public IXapValidationProvider LoadValidationProvider() {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapValidationProvider>(XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.validation", "provider"));
            } catch {
                throw;
            }
        }
    }
}

