using Xap.Evaluation.Factory.Interfaces;

namespace Xap.Evaluation.Factory.Providers {
    public interface IXapValidationProvider {
        IXapEvaluationService LoadRules<T>(T obj, string ruleType);
    }
}
