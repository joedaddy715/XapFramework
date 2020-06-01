namespace Xap.Evaluation.Factory {
    public class EvaluationFactory {
        #region "Constructors"

        private static readonly EvaluationFactory instance = new EvaluationFactory();

        static EvaluationFactory() { }

        private EvaluationFactory() { }

        public static EvaluationFactory Instance {
            get { return instance; }
        }
        #endregion
    }
}
