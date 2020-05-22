using System;
using System.Collections.Generic;
using System.Reflection;
using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Evaluation.Engine.Cache {
    public class ScriptCacheManager {
        #region "Constructors"

        private static readonly ScriptCacheManager instance = new ScriptCacheManager();

        static ScriptCacheManager() { }

        private ScriptCacheManager() { }

        public static ScriptCacheManager Instance {
            get { return instance; }
        }
        #endregion

        #region "Cache Properties"
        private ScriptOperandCache scriptOperands = ScriptOperandCache.Create();
        #endregion

        #region "Cache Methods"
        public void ExtractScriptOperands() {
            if (scriptOperands.Count == 0) {
                object lockItUp = new object();
                foreach (var item in AssemblyManager.Instance.GetLoadedAssemblies()) {
                    foreach (Type objType in item.ExportedTypes) {
                        if (((objType.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)) {

                            //See if this type implements our interface 
                            Type objInterface = objType.GetInterface("IXapEvaluationEngineOperand", true);

                            if ((objInterface != null)) {
                                IXapEvaluationEngineOperand _obj = AssemblyManager.Instance.CreateInstance<IXapEvaluationEngineOperand>("Xap.Evaluation.Engine.Rules.XapOperands");
                                lock (lockItUp) {
                                    scriptOperands.AddOperand(_obj.ToString(), new ScriptOperand(_obj.ToString(), _obj));
                                }
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<ScriptOperand> GetScriptOperands() {
           foreach(ScriptOperand operand in scriptOperands.GetOperands()) {
                yield return operand;
            }
        }
        #endregion
    }
}