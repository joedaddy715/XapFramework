namespace Xap.Evaluation.Factory.RuleSupport {
    internal class XapRuleDependent : IXapRuleDependent {
        private XapRuleDependent(string dependentName) {
            _dependentName = dependentName;
        }

        internal static XapRuleDependent Create(string dependentName) {
            return new XapRuleDependent(dependentName);
        }

        private string _dependentName = string.Empty;
        string IXapRuleDependent.DependentName {
            get { return _dependentName; }
        }
    }
}
