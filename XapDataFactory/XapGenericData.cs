using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Factory {
    public class XapGenericData : IXapGenericData {
        private XapGenericData(string displayMember, string valueMember) {
            _displayMember = displayMember;
            _valueMember = valueMember;
        }

        public static IXapGenericData Create(string displayMember, string valueMember) {
            return new XapGenericData(displayMember, valueMember);
        }

        private string _displayMember = string.Empty;
        string IXapGenericData.DisplayMember {
            get => _displayMember;
        }

        private string _valueMember = string.Empty;
        string IXapGenericData.ValueMember {
            get => _valueMember;
        }
    }
}
