using System.Data;
using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Factory {
    internal class XapDbParameter:IXapDbParameter {
        #region "Constructors"
        private XapDbParameter() { }
        private XapDbParameter(string parameterName, object parameterValue, ParameterDirection parameterDirection = ParameterDirection.Input) {
            _parameterName = $"@{parameterName}";
            _parameterValue = parameterValue;
            _parameterDirection = parameterDirection;
        }

        internal static IXapDbParameter Create() {
            return new XapDbParameter();
        }

        internal static IXapDbParameter Create(string parameterName, object parameterValue, ParameterDirection parameterDirection = ParameterDirection.Input) {
            return new XapDbParameter(parameterName, parameterValue, parameterDirection);
        }
        #endregion

        #region "Properties/Builder Methods"
        private string _parameterName = string.Empty;
        string IXapDbParameter.ParameterName {
            get => _parameterName;
            set => _parameterName = value;
        }

        private object _parameterValue = null;
        object IXapDbParameter.ParameterValue {
            get => _parameterValue;
            set => _parameterValue = value;
        }

        private ParameterDirection _parameterDirection = ParameterDirection.Input;
        ParameterDirection IXapDbParameter.ParameterDirection {
            get => _parameterDirection;
            set => _parameterDirection = value;
        }
        #endregion
    }
}
