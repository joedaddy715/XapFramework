using System.Collections.Generic;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Interfaces.Security;

namespace Xap.Infrastructure.Security {
    public class XapSecureObject:IXapSecureObject {
        #region "Constructors"
        private XapSecureObject() { }
        public static IXapSecureObject Create() {
            return new XapSecureObject();
        }
        #endregion

        #region "Properties"
        private XapCache<string, string> _secureItems = new XapCache<string, string>();
        #endregion

        #region "Public Methods"
        IEnumerable<string> IXapSecureObject.SecuredProperties() {
            foreach (KeyValuePair<string, string> kvp in _secureItems.GetItems()) {
                yield return kvp.Value; ;
            }
        }

        IXapSecureObject IXapSecureObject.AddSecureProperty(string propertyName) { 
            _secureItems.AddItem(propertyName, propertyName);
            return this;
        }

        private bool _canDelete = true;
        bool IXapSecureObject.CanDelete {
            get => _canDelete;
            set => _canDelete = value;
        }

        private bool _canInsert = true;
        bool IXapSecureObject.CanInsert {
            get => _canInsert;
            set => _canInsert = value;
        }

        private bool _canUpdate = true;
        bool IXapSecureObject.CanUpdate {
            get => _canUpdate;
            set => _canUpdate = value;
        }

        private bool _canSelect = true;
        bool IXapSecureObject.CanSelect {
            get => _canSelect;
            set => _canSelect = value;
        }
        #endregion
    }
}
