namespace Xap.Evaluation.Engine.Support {
    public class OperandResult {
        private Parser.TokenItem _result = null;
        public Parser.TokenItem Result {
            get { return _result; }
            set { _result = value; }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        private bool _success = false;
        public bool Success {
            get { return _success; }
            set { _success = value; }
        }

        public OperandResult() { }

        public OperandResult(Parser.TokenItem result, string errorMessage, bool success) {
            _result = result;
            _errorMessage = errorMessage;
            _success = success;
        }
    }
}
