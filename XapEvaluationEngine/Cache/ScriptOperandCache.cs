using System.Collections.Generic;
using Xap.Infrastructure.Caches;

namespace Xap.Evaluation.Engine.Cache {
    /// <summary>
    /// cache to create and store an instance of each evaluation engine class
    /// </summary>
    public class ScriptOperandCache  {
        #region "Constructors"
        private ScriptOperandCache() { }

        public static ScriptOperandCache Create() {
            return new ScriptOperandCache();
        }
        #endregion

        #region "Properties"
        private XapCache<string, ScriptOperand> _scriptOperands = new XapCache<string, ScriptOperand>();
        #endregion

        #region "Public Methods"
        public IEnumerable<ScriptOperand> GetOperands() {
            foreach (KeyValuePair<string, ScriptOperand> kvp in _scriptOperands.GetItems()) {
                yield return kvp.Value; ;
            }
        }

        public ScriptOperandCache AddOperand(ScriptOperand scriptOperand) {
            _scriptOperands.AddItem(scriptOperand.OperandName,scriptOperand);
            return this;
        }

        public ScriptOperandCache AddOperand(string operandName, ScriptOperand operand) {
            _scriptOperands.AddItem(operandName, operand);
            return this;
        }

        public ScriptOperand GetOperand(string operandName) {
            return _scriptOperands.GetItem(operandName);
        }

        public void Clear() {
            _scriptOperands.ClearCache();
        }

        public int Count {
            get => _scriptOperands.Count;
        }
        #endregion
    }
}
