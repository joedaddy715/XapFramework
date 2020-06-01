using System;
using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Exceptions;

namespace Xap.Validation.Factory {
    public class ValidationFactory {
        #region "Constructors"

        private static readonly ValidationFactory instance = new ValidationFactory();

        static ValidationFactory() { }

        private ValidationFactory() { }

        public static ValidationFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "Provider Loader"
        internal IXapValidationProvider LoadDataConnectionProvider(string providerType) {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapValidationProvider>(providerType);
            } catch (Exception ex) {
                throw new XapException($"Error loading validation provider {providerType}", ex);
            }
        }
        #endregion
    }
}
