using System;
using System.Collections.Generic;
using Xap.Infrastructure.Events;

namespace Xap.Infrastructure.Interfaces.Configuration {
    public interface IXapConfigurationProvider {
        #region "Load Methods"
        void LoadConfiguration();
        void LoadConfiguration(string filePath);
        void NewConfiguration();
        #endregion

        #region "reset methods"
        void RenameSection(string sectionPath, string newSectionName);
        void RenameKey(string sectionPath, string oldKeyName, string newKeyName);
        void ResetKeyValue(string sectionPath, string keyName, string newKeyValue);
        void RenameExtendedKey(string sectionPath, string keyName, string oldExtendedKeyName, string newExtendedKeyName);
        void ResetExtendedKeyValue(string sectionPath, string keyName, string extendedKeyName, string newExtKeyValue);
        #endregion

        #region "Get methods"
        bool ContainsSection(string sectionPath);
        bool ContainsKey(string sectionPath, string keyName);
        bool ContainsExtendedKey(string sectionPath, string keyName, string extKeyName);
        IEnumerable<string> GetSections(string sectionName);
        IEnumerable<string> GetKeys(string sectionName);
        IEnumerable<string> GetExtKeys(string sectionName, string keyName);
        T GetValue<T>(string sectionName, string keyName);
        T GetValue<T>(string sectionName, string keyName, string extKeyName);
        #endregion

        #region "Add Methods"
        void AddSection(string sectionPath, string newSection);
        void AddKey(string sectionPath, string keyName, string newKeyValue);
        void AddExtendedKey(string sectionPath, string keyName, string extKeyName, string extKeyValue);
        #endregion

        #region "Events"
        event EventHandler<XapEventArgs> OnSectionInserted;
        event EventHandler<XapEventArgs> OnSectionDeleted;
        event EventHandler<XapEventArgs> OnSectionChanged;
        event EventHandler<XapEventArgs> OnKeyInserted;
        event EventHandler<XapEventArgs> OnKeyDeleted;
        event EventHandler<XapEventArgs> OnKeyChanged;
        event EventHandler<XapEventArgs> OnExtKeyInserted;
        event EventHandler<XapEventArgs> OnExtKeyDeleted;
        event EventHandler<XapEventArgs> OnExtKeyChanged;
        #endregion

        #region "Dumps"
        string DumpXml();
        #endregion
    }
}
