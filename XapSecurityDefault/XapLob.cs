using Xap.Infrastructure.Interfaces.Security;

namespace Xap.Security.Default {
    internal class XapLob : IXapLob {
        private XapLob() { }

        internal static IXapLob Create() {
            return new XapLob();
        }

        private int _lobId;
        int IXapLob.LobId {
            get => _lobId;
            set => _lobId = value;
        }

        private string _lobName = string.Empty;
        string IXapLob.LobName {
            get => _lobName;
            set => _lobName = value;
        }

        private string _lobDescription = string.Empty;
        string IXapLob.LobDescription {
            get => _lobDescription;
            set => _lobDescription = value;
        }

        private bool _amosEnabled;
        bool IXapLob.AmosEnabled {
            get => _amosEnabled;
            set => _amosEnabled = value;
        }

        private bool _gmcEnabled;
        bool IXapLob.GmcEnabled {
            get => _gmcEnabled;
            set => _gmcEnabled = value;
        }

        private string _amosName = string.Empty;
        string IXapLob.AmosName {
            get => _amosName;
            set => _amosName = value;
        }

        private string _gmcName = string.Empty;
        string IXapLob.GmcName {
            get => _gmcName;
            set => _gmcName = value;
        }
    }
}
