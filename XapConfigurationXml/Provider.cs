
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Events;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Interfaces.Configuration;
using Xap.Infrastructure.Logging;

namespace Xml.Configuration {
    public class Provider : IXapConfigurationProvider {
        #region "properties"
        XmlDocument xDoc = null;
        XmlElement rootNode = null;
        private string _basePath = string.Empty;

        #endregion

        #region "Constructors"

        public Provider() { }

        #endregion

        #region "load methods"
        void IXapConfigurationProvider.NewConfiguration() {
            xDoc = new XmlDocument();
            rootNode = xDoc.CreateElement("config");
            xDoc.AppendChild(rootNode);
        }

        void IXapConfigurationProvider.LoadConfiguration() {
            try {
                LoadBaseConfiguration();
            } catch {
                throw;
            }
        }

        void IXapConfigurationProvider.LoadConfiguration(string filePath) {
            try {
                LoadBaseConfiguration(filePath);
            } catch {
                throw;
            }
        }
        #endregion

        #region "add methods"
        void IXapConfigurationProvider.AddExtendedKey(string sectionPath, string keyName, string extKeyName, string extKeyValue) {
            try {
                XmlNode parentNode = SelectKeyNode(sectionPath, keyName);
                XmlAttribute att = xDoc.CreateAttribute(extKeyName);
                att.Value = extKeyValue;
                parentNode.Attributes.Append(att);
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error adding extended key for {sectionPath}.{keyName}.{extKeyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        void IXapConfigurationProvider.AddKey(string sectionPath, string keyName, string newKeyValue) {
            try {
                XmlNode parentNode = SelectSection(sectionPath);
                XmlNode newNode = xDoc.CreateElement("key");
                XmlAttribute att = xDoc.CreateAttribute("name");
                att.Value = keyName;
                newNode.Attributes.Append(att);
                att = xDoc.CreateAttribute("value");
                att.Value = newKeyValue;
                newNode.Attributes.Append(att);
                parentNode.AppendChild(newNode);
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error adding key for {sectionPath}.{keyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        void IXapConfigurationProvider.AddSection(string sectionPath, string newSection) {
            try {
                XmlNode parentNode = SelectSection(sectionPath);
                XmlNode newNode = xDoc.CreateElement("section");
                XmlAttribute att = xDoc.CreateAttribute("name");
                att.Value = newSection;
                newNode.Attributes.Append(att);
                if (parentNode != null) {
                    parentNode.AppendChild(newNode);
                } else {
                    rootNode.AppendChild(newNode);
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error adding section for {sectionPath}.{newSection}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "get methods"
        T IXapConfigurationProvider.GetValue<T>(string sectionName, string keyName) {
            return SelectValue(sectionName, keyName).ConvertValue<T>();
        }

        T IXapConfigurationProvider.GetValue<T>(string sectionName, string keyName, string extKeyName) {
            return SelectExtendedValue(sectionName, keyName, extKeyName).ConvertValue<T>();
        }

        IEnumerable<string> IXapConfigurationProvider.GetExtKeys(string sectionPath, string keyName) {
            XmlNode keyNode = SelectKeyNode(sectionPath, keyName);
            for (int i = 0; i < keyNode.Attributes.Count; i++) {
                if (keyNode.Attributes[i].Name != "value") {
                    yield return keyNode.Attributes[i].Name;
                }
            }
        }

        IEnumerable<string> IXapConfigurationProvider.GetKeys(string sectionPath) {
            XmlNode sNode = rootNode.SelectSingleNode(SectionXPath(sectionPath));
            if (sNode == null) {
                throw new Exception($"{sectionPath} not found");
            }

            for (int i = 0; i < sNode.ChildNodes.Count; i++) {
                if (sNode.ChildNodes[i].Name == "key") {
                    yield return sNode.ChildNodes[i].Attributes.GetNamedItem("name").InnerText;
                }
            }
        }

        IEnumerable<string> IXapConfigurationProvider.GetSections(string sectionPath) {
            XmlNode tNode = rootNode.SelectSingleNode(SectionXPath(sectionPath));
            for (int i = 0; i < tNode.ChildNodes.Count; i++) {
                if (tNode.ChildNodes[i].Name == "section") {
                    XmlAttribute att = (XmlAttribute)tNode.ChildNodes[i].Attributes?.GetNamedItem("name");
                    if (att != null) {
                        yield return att.InnerText;
                    }
                }
            }
        }

        bool IXapConfigurationProvider.ContainsSection(string sectionPath) {
            try {
                XmlNode sectionNode = rootNode.SelectSingleNode(SectionXPath(sectionPath));
                if (sectionNode != null) {
                    return true;
                }
                return false;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error searching for section {sectionPath}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        bool IXapConfigurationProvider.ContainsKey(string sectionPath, string keyName) {
            try {
                XmlNode keyNode = SelectKeyNode(sectionPath, keyName);
                if (keyNode != null) {
                    return true;
                }
                return false;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error searching for key {sectionPath}.{keyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        bool IXapConfigurationProvider.ContainsExtendedKey(string sectionPath, string keyName, string extKeyName) {
            try {
                XmlNode kNode = SelectKeyNode(sectionPath, keyName);
                if (kNode != null) {
                    XmlAttribute att = kNode?.Attributes[extKeyName];
                    if (att != null) {
                        return true;
                    }
                }
                return false;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error searching for extended key {sectionPath}.{keyName}.{extKeyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "reset methods"
        void IXapConfigurationProvider.RenameExtendedKey(string sectionPath, string keyName, string oldExtendedKeyName, string newExtendedKeyName) {
            try {
                XmlNode kNode = SelectKeyNode(sectionPath, keyName);
                XmlAttribute oldAtt = null;
                for (int i = 0; i <= kNode.Attributes.Count; i++) {
                    if (kNode.Attributes[i].Name == oldExtendedKeyName) {
                        oldAtt = kNode.Attributes[i - 1];
                        break;
                    }
                }

                XmlAttribute att = (XmlAttribute)kNode.Attributes.GetNamedItem(oldExtendedKeyName);
                string value = att.Value;

                kNode.Attributes.Remove(att);
                att = xDoc.CreateAttribute(newExtendedKeyName);
                att.Value = value;

                kNode.Attributes.InsertAfter(att, oldAtt);
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error renaming extended key {sectionPath}.{keyName}.{oldExtendedKeyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        void IXapConfigurationProvider.RenameKey(string sectionPath, string oldKeyName, string newKeyName) {
            try {
                XmlAttribute att = (XmlAttribute)SelectKeyNode(sectionPath, oldKeyName).Attributes?.GetNamedItem("name");
                if (att != null) {
                    att.Value = newKeyName;
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error renaming key {sectionPath}.{oldKeyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        void IXapConfigurationProvider.RenameSection(string sectionPath, string newSectionName) {
            try {
                XmlNode sNode = SelectSection(sectionPath);
                XmlAttribute att = (XmlAttribute)sNode.Attributes?.GetNamedItem("name");
                if (att != null) {
                    att.Value = newSectionName;
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error renaming section {sectionPath}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        void IXapConfigurationProvider.ResetExtendedKeyValue(string sectionPath, string keyName, string extendedKeyName, string newExtKeyValue) {
            try {
                XmlNode kNode = SelectKeyNode(sectionPath, keyName);
                XmlAttribute att = (XmlAttribute)kNode.Attributes?.GetNamedItem(extendedKeyName);
                if (att != null) {
                    att.Value = newExtKeyValue;
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error resetting extended key value {sectionPath}.{keyName}.{extendedKeyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        void IXapConfigurationProvider.ResetKeyValue(string sectionPath, string keyName, string newKeyValue) {
            try {
                XmlNode kNode = SelectKeyNode(sectionPath, keyName);
                XmlAttribute att = (XmlAttribute)kNode.Attributes?.GetNamedItem("value");
                if (att != null) {
                    att.Value = newKeyValue;
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error resetting key value {sectionPath}.{keyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "Events"
        event EventHandler<XapEventArgs> SectionInserted;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnSectionInserted {
            add {
                if (SectionInserted != null) {
                    lock (SectionInserted) {
                        SectionInserted += value;
                    }
                } else {
                    SectionInserted = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (SectionInserted != null) {
                    lock (SectionInserted) {
                        SectionInserted -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> SectionDeleted;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnSectionDeleted {
            add {
                if (SectionDeleted != null) {
                    lock (SectionDeleted) {
                        SectionDeleted += value;
                    }
                } else {
                    SectionDeleted = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (SectionDeleted != null) {
                    lock (SectionDeleted) {
                        SectionDeleted -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> SectionChanged;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnSectionChanged {
            add {
                if (SectionChanged != null) {
                    lock (SectionChanged) {
                        SectionChanged += value;
                    }
                } else {
                    SectionChanged = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (SectionChanged != null) {
                    lock (SectionChanged) {
                        SectionChanged -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> KeyInserted;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnKeyInserted {
            add {
                if (KeyInserted != null) {
                    lock (KeyInserted) {
                        KeyInserted += value;
                    }
                } else {
                    KeyInserted = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (KeyInserted != null) {
                    lock (KeyInserted) {
                        KeyInserted -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> KeyDeleted;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnKeyDeleted {
            add {
                if (KeyDeleted != null) {
                    lock (KeyDeleted) {
                        KeyDeleted += value;
                    }
                } else {
                    KeyDeleted = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (KeyDeleted != null) {
                    lock (KeyDeleted) {
                        KeyDeleted -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> KeyChanged;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnKeyChanged {
            add {
                if (KeyChanged != null) {
                    lock (KeyChanged) {
                        KeyChanged += value;
                    }
                } else {
                    KeyChanged = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (KeyChanged != null) {
                    lock (KeyChanged) {
                        KeyChanged -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> ExtKeyInserted;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnExtKeyInserted {
            add {
                if (ExtKeyInserted != null) {
                    lock (ExtKeyInserted) {
                        ExtKeyInserted += value;
                    }
                } else {
                    ExtKeyInserted = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (ExtKeyInserted != null) {
                    lock (ExtKeyInserted) {
                        ExtKeyInserted -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> ExtKeyDeleted;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnExtKeyDeleted {
            add {
                if (ExtKeyDeleted != null) {
                    lock (ExtKeyDeleted) {
                        ExtKeyDeleted += value;
                    }
                } else {
                    ExtKeyDeleted = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (ExtKeyDeleted != null) {
                    lock (ExtKeyDeleted) {
                        ExtKeyDeleted -= value;
                    }
                }
            }
        }

        event EventHandler<XapEventArgs> ExtKeyChanged;
        event EventHandler<XapEventArgs> IXapConfigurationProvider.OnExtKeyChanged {
            add {
                if (ExtKeyChanged != null) {
                    lock (ExtKeyChanged) {
                        ExtKeyChanged += value;
                    }
                } else {
                    ExtKeyChanged = new EventHandler<XapEventArgs>(value);
                }
            }

            remove {
                if (ExtKeyChanged != null) {
                    lock (ExtKeyChanged) {
                        ExtKeyChanged -= value;
                    }
                }
            }
        }
        #endregion

        #region "default configuration loading methods"
        private void LoadBaseConfiguration() {
            try {
                xDoc = new XmlDocument();
                string path = XapEnvironment.Instance.ConfigurationFile;
                path = XapEnvironment.Instance.MapFolderPath(path);

                if (File.Exists(path)) {
                    xDoc.Load(path);
                    rootNode = (XmlElement)xDoc.SelectSingleNode("config");
                    if (rootNode == null) {
                        throw new Exception("Invalid Configuration File");
                    }
                    _basePath = rootNode?.SelectSingleNode($"section[@name='{XapEnvironment.Instance.EnvironmentName}']/key[@name='basePath']").Attributes.GetNamedItem("value").InnerText;
                } else {
                    throw new Exception("Configuration File Not Found");
                }
            } catch {
                throw new Exception("Error loading base configuration");
            }
        }

        private void LoadBaseConfiguration(string filePath) {
            try {
                xDoc = new XmlDocument();
                filePath = XapEnvironment.Instance.MapFolderPath(filePath);

                if (File.Exists(filePath)) {
                    xDoc.Load(filePath);
                    rootNode = (XmlElement)xDoc.SelectSingleNode("config");
                    if (rootNode == null) {
                        throw new Exception("Invalid Base Configuration File");
                    }
                    _basePath = rootNode?.SelectSingleNode($"section[@name='{XapEnvironment.Instance.EnvironmentName}']/key[@name='basePath']").Attributes.GetNamedItem("value").InnerText;
                } else {
                    throw new Exception("Configuration File Not Found");
                }
            } catch {
                throw new Exception("Error loading base configuration");
            }
        }

        private string ReplaceBasePath(string keyValue) {
            try {
                if (keyValue.Contains("$basePath$")) {
                    keyValue = keyValue?.Replace("$basePath$", _basePath);
                }
                return keyValue;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error replacing base path {keyValue}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "Xpath helpers"
        private string SectionXPath(string sectionPath) {
            if (string.IsNullOrWhiteSpace(sectionPath)) {
                sectionPath = "config";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string[] _sections = System.Text.RegularExpressions.Regex.Split(sectionPath, @"\.");
            string txt = string.Empty;

            for (int i = 0; i < _sections.Length; i++) {
                sb.Append($"section[@name='{_sections[i]}']/");
            }

            txt = sb.ToString();
            if (txt.Length > 0) {
                txt = txt.Remove(txt.Length - 1, 1);
            }
            return txt;
        }

        private XmlNode SelectSection(string sectionPath) {
            try {
                XmlNode sNode = rootNode.SelectSingleNode(SectionXPath(sectionPath));
                if (sNode == null) {
                    return rootNode;
                }
                return sNode;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"{sectionPath} not found");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private XmlNode SelectKeyNode(string sectionPath, string keyName) {
            try {
                XmlNode kNode = rootNode.SelectSingleNode($"{SectionXPath(sectionPath)}/key[@name='{keyName}']");
                return kNode;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"{sectionPath}.{keyName} not found");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private string SelectValue(string sectionPath, string keyName) {
            try {
                XmlAttribute att = (XmlAttribute)SelectKeyNode(sectionPath, keyName).Attributes?.GetNamedItem("value");
                if (att == null) {
                    throw new Exception($"{sectionPath}.{keyName} not found");
                }
                att.InnerText = ReplaceBasePath(att.InnerText);
                return att.InnerText;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"{sectionPath}.{keyName} not found");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private string SelectExtendedValue(string sectionPath, string keyName, string extKeyName) {
            try {
                XmlAttribute att = (XmlAttribute)SelectKeyNode(sectionPath, keyName).Attributes?.GetNamedItem(extKeyName);
                if (att == null) {
                    throw new Exception($"{sectionPath}.{keyName}.{extKeyName} not found");
                }
                att.InnerText = ReplaceBasePath(att.InnerText);
                return att.InnerText;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"{sectionPath}.{keyName}.{extKeyName} not found");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
        #endregion

        #region "Dumps"
        string IXapConfigurationProvider.DumpXml() {
            return xDoc.InnerXml;
        }
        #endregion
    }
}
