namespace Xap.Infrastructure.Attributes {
    public class UiBinding : System.Attribute {
        private string _controlName = string.Empty;
        public string ControlName {
            get { return _controlName; }
            set { _controlName = value; }
        }

        public UiBinding(string controlName) {
            _controlName = controlName;
        }
    }
}
