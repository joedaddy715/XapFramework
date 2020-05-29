using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;

namespace Xap.Infrastructure.AppDomain
{
    public class AssemblyManager {
        static readonly object propLocksDog = new object();
        private XapCache<string, Assembly> _loadedAssemblies = new XapCache<string, Assembly>();
        private XapCache<string, string> _loadedAssemblyClass = new XapCache<string, string>();

        private static readonly AssemblyManager instance = new AssemblyManager();

        static AssemblyManager() { }

        private AssemblyManager() { }

        public static AssemblyManager Instance {
            get { return instance; }
        }

        public void ClearCache() {
            _loadedAssemblies.ClearCache();
            _loadedAssemblyClass.ClearCache();
        }

        public IEnumerable<Assembly> GetLoadedAssemblies() {
            foreach (KeyValuePair<string, Assembly> kvp in _loadedAssemblies.GetItems()) {
                yield return kvp.Value;
            }
        }

        public void LoadAssemblies(string assemblyPath, string interfaceName) {
            AssemblyLoader assemblyLoader = new AssemblyLoader();
            Type objInterface;
            try {
                string[] files = Directory.GetFiles(assemblyPath);

                foreach (string file in files) {
                    if (file.EndsWith("dll")) {
                        string fileName = Path.GetFileNameWithoutExtension(file);

                        Assembly _assembly = assemblyLoader.LoadAssembly(LoadMethod.LoadFile, file);

                        foreach (Type objType in _assembly.ExportedTypes) {
                            if (((objType.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)) {
                                //See if this type implements our interface
                                objInterface = objType.GetInterface(interfaceName, true);
                                if ((objInterface != null)) {
                                    if (_loadedAssemblies.GetItem(_assembly.FullName) == null) {
                                        lock (propLocksDog) {
                                            _loadedAssemblies.AddItem(_assembly.FullName, _assembly);
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(_loadedAssemblyClass.GetItem(objType.FullName))) {
                                        lock (propLocksDog) {
                                            _loadedAssemblyClass.AddItem(objType.FullName, _assembly.FullName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw new XapException($"Error loading assemblies from {assemblyPath} for {interfaceName}", ex);
            }
        }

        public void LoadAssembly(string assemblyPath, string assemblyName, string interfaceName) {
            AssemblyLoader assemblyLoader = new AssemblyLoader();
            Type objInterface;
            try {
                string file = $"{assemblyPath}{assemblyName}.dll";
                if (File.Exists(file)) {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    Assembly _assembly = assemblyLoader.LoadAssembly(LoadMethod.LoadFile, file);
                    foreach (Type objType in _assembly.ExportedTypes) {
                        if (((objType.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)) {

                            //See if this type implements our interface 
                            objInterface = objType.GetInterface(interfaceName, true);

                            if ((objInterface != null)) {
                                if (objInterface.Name == interfaceName) {
                                    if (_loadedAssemblies.GetItem(_assembly.FullName) == null) {
                                        lock (propLocksDog) {
                                            _loadedAssemblies.AddItem(_assembly.FullName, _assembly);
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(_loadedAssemblyClass.GetItem(objType.FullName))) {
                                        lock (propLocksDog) {
                                            _loadedAssemblyClass.AddItem(objType.FullName, _assembly.FullName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    throw new FileNotFoundException($"{assemblyPath}{assemblyName} Not Found");
                }
            } catch (Exception ex) {
                throw new XapException($"Error loading assemblies from {assemblyPath} for assembly {assemblyName} and {interfaceName}", ex);
            }
        }

        public T CreateInstance<T>(string typeName) {
            try {
                if (!string.IsNullOrWhiteSpace(_loadedAssemblyClass.GetItem(typeName))) {
                    if (_loadedAssemblies.GetItem(_loadedAssemblyClass.GetItem(typeName)) != null) {
                        Assembly newAssembly = _loadedAssemblies.GetItem(_loadedAssemblyClass.GetItem(typeName));
                        return (T)newAssembly.CreateInstance(typeName);
                    }
                }
                throw new ArgumentException($"No matching type found for {typeName}");
            } catch (Exception ex) {
                throw new XapException($"Error creating an instance of {typeName}", ex);
            }
        }
    }
}
