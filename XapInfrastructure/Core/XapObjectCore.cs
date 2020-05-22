using System;
using System.ComponentModel;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Events;

namespace Xap.Infrastructure.Core {
    [Serializable]
    public abstract class XapObjectCore {
        #region "Constructors"
        public XapObjectCore() {

        }
        #endregion

        #region "PropertyNotification"
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Sends a notification that the property value has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName, object valueToAssign) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region "properties"
        private PropertyCache _propertyCache = PropertyCache.Create();
        public PropertyCache Properties {
            get { return _propertyCache; }
            set { _propertyCache = value; }
        }

        private bool _isDirty = false;
        public bool IsDirty {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        private bool _isValid = true;
        public bool IsValid {
            get { return _isValid; }
            set { _isValid = value; }
        }

        private bool _isNew = true;
        public bool IsNew {
            get { return _isNew; }
            set { _isNew = value; }
        }

        private bool _isLoading = false;
        public bool IsLoading {
            get { return _isLoading; }
            set { _isLoading = value; }
        }
        #endregion

        #region "Events"
        public delegate void Object_Inserted(object sender, XapEventArgs e);
        /// <summary>
        /// Sends a notification that a successful database insert has occurred
        /// </summary>
        public event Object_Inserted OnObjectInserted;
        protected void OnInserted(XapEventArgs e) {
            OnObjectInserted?.Invoke(this, e);
        }

        public delegate void Object_Updated(object sender, XapEventArgs e);
        /// <summary>
        /// Sends a notification that a successful database update has occurred
        /// </summary>
        public event Object_Updated OnObjectUpdated;
        protected void OnUpdated(XapEventArgs e) {
            OnObjectUpdated?.Invoke(this, e);
        }

        public delegate void Object_Deleted(object sender, XapEventArgs e);
        /// <summary>
        /// Sends a notification that a successfull database delete has occurred
        /// </summary>
        public event Object_Deleted OnObjectDeleted;
        protected void OnDeleted(XapEventArgs e) {
            OnObjectDeleted?.Invoke(this, e);
        }

        public delegate void Object_StatusChange(object sender, XapEventArgs e);
        /// <summary>
        /// Sends a notification that the objects state has changed
        /// </summary>
        public event Object_StatusChange OnObjectStatusChange;
        protected void OnStatusChange(XapEventArgs e) {
            OnObjectStatusChange?.Invoke(this, e);
        }

        public delegate void Property_IsInvalid(object sender, XapEventArgs e);
        /// <summary>
        /// Sends a notification that the property value is invalid
        /// </summary>
        public event Property_IsInvalid OnPropertyInvalid;
        protected void OnObjectPropertyInvalid(XapEventArgs e) {
            OnPropertyInvalid?.Invoke(this, e);
        }

        public delegate void Object_IsInvalid(object sender, XapEventArgs e);
        /// <summary>
        /// Sends a notification that the object is in an invalid state
        /// </summary>
        public event Object_IsInvalid OnObjectInvalid;
        protected void OnObjectIsInvalid(XapEventArgs e) {
            OnObjectInvalid?.Invoke(this, e);
        }
        #endregion

    }
}
