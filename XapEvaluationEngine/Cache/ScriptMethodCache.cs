using System.Collections.Generic;
using System.Reflection;
using Xap.Infrastructure.Caches;

namespace Xap.Evaluation.Engine.Cache {
    /// <summary>
    /// Class to store the methods loaded from the Operand libraries
    /// </summary>
    public class ScriptMethodCache  {
        #region "Constructors"
        private ScriptMethodCache() { }

        public static ScriptMethodCache Create() {
            return new ScriptMethodCache();
        }
        #endregion

        #region "Properties"
        private XapCache<string, MethodInfo> _scriptMethods = new XapCache<string, MethodInfo>();
        #endregion

        #region "Public Methods"
        public IEnumerable<MethodInfo> GetScriptMethods() {
            foreach (KeyValuePair<string, MethodInfo> kvp in _scriptMethods.GetItems()) {
                yield return kvp.Value; ;
            }
        }

        public ScriptMethodCache AddMethod(MethodInfo method) {
            _scriptMethods.AddItem(method.Name, method);
            return this;
        }

        public ScriptMethodCache AddMethod(string methodName, MethodInfo method) {
            _scriptMethods.AddItem(methodName, method);
            return this;
        }

        public MethodInfo GetScriptMethod(string methodName) {
            return _scriptMethods.GetItem(methodName);
        }

        public void Clear() {
            _scriptMethods.ClearCache();
        }

        public int Count {
            get => _scriptMethods.Count;
        }
        #endregion

    }
}
