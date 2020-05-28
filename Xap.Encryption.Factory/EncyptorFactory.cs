using Xap.Encryption.Factory.Interfaces;
using Xap.Infrastructure.AppDomain;

namespace Xap.Encryption.Factory {
    public class EncyptorFactory  {
        #region "Constructors"

        private static readonly EncyptorFactory instance = new EncyptorFactory();

        static EncyptorFactory() { }

        private EncyptorFactory() { }

        public static EncyptorFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "Provider Loader"
        public IXapEncryptionProvider LoadEncryptionProvider(string encryptionType) {
            return AssemblyManager.Instance.CreateInstance<IXapEncryptionProvider>(encryptionType);
        }
        #endregion
    }
}
