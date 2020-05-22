using System.Collections.Generic;
using System.Reflection;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Evaluation.Engine.Cache {
    public class ScriptOperand {
        public ScriptOperand(string operandName,IXapEvaluationEngineOperand engineOperand) {
            _operandName = operandName;
            _engineOperand = engineOperand;
            ExtractOperandMethods();
        }

        private string _operandName = string.Empty;
        public string OperandName {
            get { return _operandName; }
        }

        private IXapEvaluationEngineOperand _engineOperand;
        public IXapEvaluationEngineOperand EngineOperand {
            get { return _engineOperand; }
        }

        private ScriptMethodCache engineOperandMethods = ScriptMethodCache.Create();
        
        private void ExtractOperandMethods() {
            MethodInfo[] _methodInfos = _engineOperand.GetType().GetMethods();
            foreach (MethodInfo _methodInfo in _methodInfos) {
                engineOperandMethods.AddMethod(_methodInfo.Name, _methodInfo);
            }
        }

        public IEnumerable<MethodInfo> GetEngineOperandMethods() {
            foreach(MethodInfo method in engineOperandMethods.GetScriptMethods()) {
                yield return method;
            }
        }
    }
}
