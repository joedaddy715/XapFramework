using System;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Evaluation.Engine.RuleSupport {
    [Serializable]
    public class XapRuleDependent:IXapRuleDependent {
        private XapRuleDependent(string dependentName) {
            _dependentName = dependentName;
        }

        public static XapRuleDependent Create(string dependentName) {
            return new XapRuleDependent(dependentName);
        }

        private string _dependentName = string.Empty;
        string IXapRuleDependent.DependentName {
            get { return _dependentName; }
        }
    }
}
