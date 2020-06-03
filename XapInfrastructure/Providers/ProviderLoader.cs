using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Interfaces.Security;

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

        public IXapSecurityProvider LoadSecurityProvider() {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapSecurityProvider>(XapConfig.Instance.GetValue<string>($"{XapEnvironment.Instance.EnvironmentName}.security", "provider"));
            } catch {
                throw;
            }
        }
    }
}

