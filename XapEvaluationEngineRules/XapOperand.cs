using System;
using System.Text;
using System.Text.RegularExpressions;
using Xap.Data.Factory;
using Xap.Data.Factory.Interfaces;
using Xap.Evaluation.Engine.Parser;
using Xap.Evaluation.Engine.Support;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Logging.Factory;

namespace Xap.Evaluation.Engine.Rules {
    public class XapOperands : IXapEvaluationEngineOperand {
        #region "Operands"
        public Xap.Evaluation.Engine.Support.OperandResult SubString(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "SubString requires 3 parameters", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string _str = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                if (_str.TrimEnd().Length == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, String.Format("SubString Operand Function requires an integer as the second parameter.: {0}", Parameters[0].TokenName.ToString()), false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[1].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, String.Format("SubString Operand Function requires an integer as the second parameter.: {0}", Parameters[1].TokenName.ToString()), false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[2].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, String.Format("SubString Operand Function requires an integer as the third parameter.: {0}", Parameters[2].TokenName.ToString()), false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                _str = _str.Substring(int.Parse(Parameters[1].TokenName), int.Parse(Parameters[2].TokenName));
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(_str, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);

                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult UCase(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "UCase[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string _str = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                _str = _str.ToUpper();
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(_str, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);

                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult LCase(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "LCase[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string _str = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                _str = _str.ToLower();
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(_str, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);

                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Abs(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Abs[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take a abs of an item that can be converted to a double
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(Parameters[0].TokenName) == true) {
                    double temp = Parameters[0].TokenName_Double;
                    double abs_temp = Math.Abs(temp);
                } else {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Abs[]: Operand Function can only evaluate parameters that can be converted to a double.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Avg(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Avg[]: Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                double total = 0;
                try {
                    foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                        if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(tItem.TokenName) == true) {
                            total += tItem.TokenName_Double;
                        } else {
                            operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Avg[]: Operand Function can only calculate the average of parameters that can be converted to double.", false);
                            XapLogger.Instance.Error(operandResult.ErrorMessage);
                            return operandResult;
                        }
                    }
                } catch (Exception err) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Avg[]: " + err.Message, false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                double dAvg = 0;
                try {
                    dAvg = total / Convert.ToDouble(Parameters.Count);
                } catch (Exception err) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Avg[] while calcuating the average: " + err.Message, false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(dAvg.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Between(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Between[]: Operand Function requires 3 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // all 3 parameters must be able to convert to double
                for (int i = 0; i < 3; i++) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(Parameters[0].TokenName) == false) {
                        operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Between[] Operand Function requires 3 parameter that can be converted to double.", false);
                        XapLogger.Instance.Error(operandResult.ErrorMessage);
                        return operandResult;
                    }
                }

                // get the 3 doubles
                double d1 = Parameters[0].TokenName_Double;
                double d2 = Parameters[1].TokenName_Double;
                double d3 = Parameters[2].TokenName_Double;

                // double result
                bool result = false;
                if (d1 >= d2) {
                    if (d1 <= d3) {
                        result = true;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(result.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult ConCat(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "ConCat[]: Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string conCatString = "\"";
                try {
                    foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                        conCatString += Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(tItem.TokenName);
                    }
                } catch (Exception err) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function ConCat[]: " + err.Message, false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                conCatString += "\"";

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(conCatString, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IIF(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {

                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "IIF[]: Operand Function requires 3 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the first parameter must be a boolean
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsBoolean(Parameters[0].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function IIF[]: Operand Function requires the first paraemter to be a boolean.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                if (Parameters[0].TokenName_Boolean == true) {
                    // return the first parameter
                    Result = Parameters[1];
                } else {
                    // return the second parameter
                    Result = Parameters[2];
                }

                //Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(conCatString, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Left(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {

                if (Parameters.Count != 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Left[]: Operand Function requires 2 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                bool isText = Xap.Evaluation.Engine.Support.DataTypeCheck.IsText(Parameters[0].TokenName);
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                // the second parameter must be an integer
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[1].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Left[]: Operand Function requires an integer as the second parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                int leftAmount = Parameters[1].TokenName_Int;

                // the left amount must be less than or equal to the length of the string
                if (leftAmount > tempToken.Length) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Left[]: second parameter can not be greater than the length of the string", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string newValue = tempToken.Substring(0, leftAmount);
                if (isText == true) newValue = "\"" + newValue + "\"";

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(newValue, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Len(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Len[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(tempToken.Length.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Mid(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Mid[] Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the mid of items that can be convert to double
                double[] arrData = new double[Parameters.Count];
                int index = 0;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(tItem.TokenName) == true) {
                        arrData[index] = tItem.TokenName_Double;
                        index++;
                    } else {
                        operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Mid[] Operand Function can only calculate the middle of parameters that can be converted to double.", false);
                        XapLogger.Instance.Error(operandResult.ErrorMessage);
                        return operandResult;
                    }
                }

                // sort the array of doubles
                Array.Sort<double>(arrData);

                double mid = 0;
                double midDBLItem = ((arrData.Length + 1) / 2) - 1;
                int midItem = Convert.ToInt32(Math.Floor(midDBLItem));

                if ((arrData.Length % 2) == 0) {
                    // there is an even number of items in the array
                    double item1 = arrData[midItem];
                    double item2 = arrData[midItem + 1];

                    mid = (item1 + item2) / 2;
                } else {
                    // there is an odd number of items in the array.
                    mid = arrData[midItem];
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(mid.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Right(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Right[] Operand Function requires 2 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                bool isText = Xap.Evaluation.Engine.Support.DataTypeCheck.IsText(Parameters[0].TokenName);
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                // the second parameter must be an integer
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[1].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Right[] Operand Function requires an integer as the second parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                int rightAmount = Parameters[1].TokenName_Int;

                // the left amount must be less than or equal to the length of the string
                if (rightAmount > tempToken.Length) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Right[] Operand Function requires an integer as the second parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string newValue = tempToken.Substring(tempToken.Length - rightAmount, rightAmount);
                if (isText == true) newValue = "\"" + newValue + "\"";

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(newValue, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Round(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Round[] Operand Function requires 2 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the first parameters must be a double
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(Parameters[0].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Round[] Operand Function requires the first parameter to be a double.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the second parameter must be a integer
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[1].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Round[] Operand Function requires the second parameter to be a integer.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                double roundItem = Parameters[0].TokenName_Double;
                int roundAmt = Parameters[1].TokenName_Int;
                if (roundAmt < 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Round[] Operand Function requires the second parameter to be a positive integer.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                double final = Math.Round(roundItem, roundAmt);

                string format = "#";
                if (roundAmt > 0) {
                    format += ".";
                    for (int i = 0; i < roundAmt; i++) format += "#";
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(final.ToString(format), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Sqrt(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Sqrt[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take a abs of an item that can be converted to a double
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(Parameters[0].TokenName) == true) {
                    double temp = Parameters[0].TokenName_Double;
                    double sqrt_temp = Math.Sqrt(temp);

                    Result = new Xap.Evaluation.Engine.Parser.TokenItem(sqrt_temp.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                } else {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Sqrt[] can only evaluate parameters that can be converted to a double.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                //Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IsNullOrEmpty(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "IsNullOrEmpty[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // check if the parameter is null;
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsNULL(Parameters[0].TokenName) == true)
                    Result = new Xap.Evaluation.Engine.Parser.TokenItem("true", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                else {
                    string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                    tempToken = tempToken.Trim().ToLower();

                    if (tempToken == "null")
                        Result = new Xap.Evaluation.Engine.Parser.TokenItem("true", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                    else {
                        bool final = String.IsNullOrEmpty(tempToken);
                        Result = new Xap.Evaluation.Engine.Parser.TokenItem(final.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                    }
                }

                //Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IsTrueOrNull(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "IsTrueOrNull[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                tempToken = tempToken.Trim();

                bool isTrueOrNull = true;
                if (String.IsNullOrEmpty(tempToken) == false) {
                    isTrueOrNull = (tempToken.ToLower() == "true");
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(isTrueOrNull.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IsFalseOrNull(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "IsFalseOrNull[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                tempToken = tempToken.Trim();

                bool isfalseOrNull = true;
                if (String.IsNullOrEmpty(tempToken) == false) {
                    isfalseOrNull = (tempToken.ToLower() == "false");
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(isfalseOrNull.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Trim(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Trim[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                bool isText = Xap.Evaluation.Engine.Support.DataTypeCheck.IsText(Parameters[0].TokenName);
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                string newValue = tempToken.Trim();
                if (isText == true) newValue = "\"" + newValue + "\"";

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(newValue, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult RTrim(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {

                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "RTrim[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                bool isText = Xap.Evaluation.Engine.Support.DataTypeCheck.IsText(Parameters[0].TokenName);
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                string newValue = tempToken.TrimEnd();
                if (isText == true) newValue = "\"" + newValue + "\"";

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(newValue, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult LTrim(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "LTrim[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                bool isText = Xap.Evaluation.Engine.Support.DataTypeCheck.IsText(Parameters[0].TokenName);
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                string newValue = tempToken.TrimStart();
                if (isText == true) newValue = "\"" + newValue + "\"";

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(newValue, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult DateDiff(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateDiff[] Operand Function requires 2 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                DateTime startDate;
                if (!DateTime.TryParse(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName), out startDate)) {
                }

                DateTime endDate;
                if (!DateTime.TryParse(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName), out endDate)) {
                }

                TimeSpan retVal = endDate - startDate;

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal.Days.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult DateAdd(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count < 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateAdd[] Operand Function requires 3 parameters", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the first parameter must be a date
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(Parameters[0].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateAdd[] Operand Function requires a date in the first parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the second parameter must be a "d", "m","b","bh" or "y";
                string dateAddType = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName);
                dateAddType = dateAddType.Trim().ToLower();

                if (dateAddType != "d") {
                    if (dateAddType != "m") {
                        if (dateAddType != "y") {
                            if (dateAddType != "b") {
                                if (dateAddType != "bh") {
                                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateAdd[] Operand Function requires that the second parameter is a d, m, y, b,bh.", false);
                                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                                    return operandResult;
                                }
                            }
                        }
                    }
                }

                // the last parameter must be an integer
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[2].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateAdd[] Operand Function requires an integer in the third parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }
                int dateAmt = Parameters[2].TokenName_Int;
                int additionalDays;

                // get the data value
                DateTime dateValue = Parameters[0].TokenName_DateTime;

                if (dateAddType == "d") {
                    dateValue = dateValue.AddDays(dateAmt);
                } else if (dateAddType == "m") {
                    dateValue = dateValue.AddMonths(dateAmt);
                } else if (dateAddType == "y") {
                    dateValue = dateValue.AddYears(dateAmt);
                } else if (dateAddType == "b") {
                    int numAdded = 0;

                    DateTime tempDate = dateValue;
                    while (numAdded < dateAmt) {
                        tempDate = tempDate.AddDays(1);
                        if (tempDate.DayOfWeek != DayOfWeek.Saturday) {
                            if (tempDate.DayOfWeek != DayOfWeek.Sunday) {
                                numAdded++;
                                dateValue = tempDate;
                            }
                        }
                    }
                } else if (dateAddType == "bh") {
                    if (Parameters.Count == 4) {
                        DateTime tempDate = dateValue;
                        string holidaysParam = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[3].TokenName);
                        string[] _holidays = holidaysParam.Split('|');
                        for (int i = 0; i < dateAmt; i++) {
                            tempDate = tempDate.AddDays(1);
                            if (_holidays != null) {
                                while (IsHoliday(tempDate, _holidays) || IsWeekEnd(tempDate)) {
                                    tempDate = tempDate.AddDays(1);
                                }
                            }
                        }
                        dateValue = tempDate;
                    }

                }
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(dateValue.ToString("MM/dd/yyyy"), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Date, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult DaysBetween(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            bool retVal = false;
            try {
                if (Parameters.Count < 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DaysBetween[] Operand Function requires at least 3 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the first parameter must the start date
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(Parameters[0].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DaysBetween[] Operand Function requires a start date in the first parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the second parameter must be the end date
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(Parameters[1].TokenName) == false) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DaysBetween[] Operand Function requires a end date in the second parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // the third parameter must be a "d", "m","b","bh" or "y";
                string dateAddType = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[2].TokenName);
                dateAddType = dateAddType.Trim().ToLower();

                if (dateAddType != "d") {
                    if (dateAddType != "b") {
                        if (dateAddType != "bh") {
                            operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DaysBetween[] Operand Function requires that the third parameter is a d,b,bh.", false);
                            XapLogger.Instance.Error(operandResult.ErrorMessage);
                            return operandResult;
                        }
                    }
                }

                DateTime _startDate = DateTime.Parse(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName));
                DateTime _endDate = DateTime.Parse(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName));
                int numDays = 0;

                if (dateAddType == "d") {
                    while(_startDate <= _endDate) {
                        numDays++;
                        _startDate = _startDate.AddDays(1);
                    }
                } else if (dateAddType == "b") {
                    while(_startDate <= _endDate) {
                        if (!IsWeekEnd(_startDate)) {
                            numDays++;
                        }
                        _startDate = _startDate.AddDays(1);
                    }
                } else if (dateAddType == "bh") {
                    if (Parameters.Count == 4) {
                        string[] _holidays = Parameters[3].TokenName.Split('|');
                        while (_startDate <= _endDate) {
                            if(!IsHoliday(_startDate,_holidays) && !IsWeekEnd(_startDate)) {
                                numDays++;
                            }
                            _startDate = _startDate.AddDays(1);
                        }
                    }

                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(numDays.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult RPad(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "RPad[] Operand Function requires 3 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string padText = DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                string padString = DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName);

                // the last parameter must be an integer
                if (DataTypeCheck.IsInteger(Parameters[2].TokenName) == false) {
                    operandResult = new OperandResult(null, "RPad[] Operand Function requires the 3rd parameter to be an integer.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                int padCount = Parameters[2].TokenName_Int;

                string finalPad = padText;
                for (int i = 0; i < padCount; i++) finalPad += padString;

                Xap.Evaluation.Engine.Parser.TokenItem Result = new TokenItem("\"" + finalPad + "\"", TokenType.Token_Operand, TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult LPad(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "LPad[] Operand Function requires 3 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string padText = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                string padString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName);

                // the last parameter must be an integer
                if (DataTypeCheck.IsInteger(Parameters[2].TokenName) == false) {
                    operandResult = new OperandResult(null, "LPad[] Operand Function requires the 3rd parameter to be an integer.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                int padCount = Parameters[2].TokenName_Int;

                string finalPad = "";
                for (int i = 0; i < padCount; i++) finalPad += padString;

                finalPad += padText;

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem("\"" + finalPad + "\"", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Join(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count < 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Join[] Operand Function requires at least 2 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the join delimiter
                string joinString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                // loop through the items
                //string finalJoin = "";
                StringBuilder finalJoin = new StringBuilder();

                for (int i = 1; i < Parameters.Count; i++) {
                    //finalJoin += Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[i].TokenName);
                    finalJoin.Append(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[i].TokenName));
                    //if (i != Parameters.Count - 1) finalJoin += joinString;
                    if (i != Parameters.Count - 1) finalJoin.Append(joinString);
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem("'" + finalJoin.ToString() + "'", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult SearchString(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "SearchString[] Operand Function requires 3 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                // the second parameter must be an integer
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsInteger(Parameters[1].TokenName) == false) {
                    operandResult = new OperandResult(null, "SearchString[] Operand Function requires an integer as the second parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }
                int startPosition = Parameters[1].TokenName_Int;

                // the last parameter must be an string
                string searchFor = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[2].TokenName);

                int location = tempToken.IndexOf(searchFor, startPosition);

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(location.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Day(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Day[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take a Day of an item that can be converted to a date
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(Parameters[0].TokenName) == true) {
                    int day = Parameters[0].TokenName_DateTime.Day;
                    Result = new Xap.Evaluation.Engine.Parser.TokenItem(day.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                } else {
                    operandResult = new OperandResult(null, "Error in operand function Day[]: Operand Function requires the parameter of type date time.", false);
                }

                //Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Month(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Month[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take a Day of an item that can be converted to a date
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(Parameters[0].TokenName) == true) {
                    int month = Parameters[0].TokenName_DateTime.Month;
                    Result = new Xap.Evaluation.Engine.Parser.TokenItem(month.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                } else {
                    operandResult = new OperandResult(null, "Error in operand function Avg[]: Operand Function requires 1 parameter that can be converted to a date time.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                //Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Year(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Year[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take a Day of an item that can be converted to a date
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(Parameters[0].TokenName) == true) {
                    int year = Parameters[0].TokenName_DateTime.Year;
                    Result = new Xap.Evaluation.Engine.Parser.TokenItem(year.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                } else {
                    operandResult = new OperandResult(null, "Error in operand function Year[]: Operand Function requires 1 parameter that can be converted to date time.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                //Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult NumericMax(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function NumericMax[]: Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                double dblMax = 0;
                bool firstItem = true;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(tItem.TokenName) == true) {
                        if (firstItem == true) {
                            dblMax = tItem.TokenName_Double;
                            firstItem = false;
                        } else {
                            if (tItem.TokenName_Double > dblMax) dblMax = tItem.TokenName_Double;
                        }
                    } else {
                        operandResult = new OperandResult(null, "Error in operand function NumericMax[]: Operand Function expects that all parameters can be converted to double.", false);
                        XapLogger.Instance.Error(operandResult.ErrorMessage);
                        return operandResult;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(dblMax.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult NumericMin(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function NumericMin[]: Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                double dblMin = 0;
                bool firstItem = true;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDouble(tItem.TokenName) == true) {
                        if (firstItem == true) {
                            dblMin = tItem.TokenName_Double;
                            firstItem = false;
                        } else {
                            if (tItem.TokenName_Double < dblMin) dblMin = tItem.TokenName_Double;
                        }
                    } else {
                        operandResult = new OperandResult(null, "Error in operand function NumericMin[]: Operand Function expects that all parameters can be converted to double.", false);
                        XapLogger.Instance.Error(operandResult.ErrorMessage);
                        return operandResult;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(dblMin.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult DateMax(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateMax[] Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                DateTime maxDate = DateTime.MinValue;
                bool firstItem = true;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(tItem.TokenName) == true) {
                        if (firstItem == true) {
                            maxDate = tItem.TokenName_DateTime;
                            firstItem = false;
                        } else {
                            TimeSpan ts = maxDate.Subtract(tItem.TokenName_DateTime);
                            if (ts.TotalDays < 0) maxDate = tItem.TokenName_DateTime;
                        }
                    } else {
                        operandResult = new OperandResult(null, "DateMax[] Operand Function expects that all parameters can be converted to date time.", false);
                        XapLogger.Instance.Error(operandResult.ErrorMessage);
                        return operandResult;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(maxDate.ToString("M.d.yyyy"), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Date, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult DateMin(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "DateMin[] Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                DateTime minDate = DateTime.MinValue;
                bool firstItem = true;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsDate(tItem.TokenName) == true) {
                        if (firstItem == true) {
                            minDate = tItem.TokenName_DateTime;
                            firstItem = false;
                        } else {
                            TimeSpan ts = minDate.Subtract(tItem.TokenName_DateTime);
                            if (ts.TotalDays > 0) minDate = tItem.TokenName_DateTime;
                        }
                    } else {
                        operandResult = new OperandResult(null, "DateMin[] Operand Function expects that all parameters can be converted to date time.", false);
                        XapLogger.Instance.Error(operandResult.ErrorMessage);
                        return operandResult;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(minDate.ToString("M.d.yyyy"), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Date, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult StringMax(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "StringMax[] Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                string maxString = "";
                bool firstItem = true;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (firstItem == true) {
                        maxString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(tItem.TokenName);
                        firstItem = false;
                    } else {
                        string currString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(tItem.TokenName);
                        if (maxString.CompareTo(currString) < 0) maxString = currString;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem("\"" + maxString + "\"", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult StringMin(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "StringMin[] Operand Function requires at least 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take the average of items that can be convert to double
                string minString = "";
                bool firstItem = true;
                foreach (Xap.Evaluation.Engine.Parser.TokenItem tItem in Parameters) {
                    if (firstItem == true) {
                        minString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(tItem.TokenName);
                        firstItem = false;
                    } else {
                        string currString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(tItem.TokenName);
                        if (minString.CompareTo(currString) > 0) minString = currString;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem("\"" + minString + "\"", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Contains(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count <= 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Contains[] Operand Function requires at least 2 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the search item
                string searchString = Parameters[0].TokenName;

                // loop through the items
                bool foundItem = false;
                for (int i = 1; i < Parameters.Count; i++) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[i].TokenName) == searchString) {
                        foundItem = true;
                        break;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(foundItem.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IndexOf(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count <= 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "IndexOf[] Operand Function requires at least 2 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the search item
                string searchString = Parameters[0].TokenName;

                // loop through the items
                int index = -1;
                for (int i = 1; i < Parameters.Count; i++) {
                    if (Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[i].TokenName) == searchString) {
                        index = i - 1;
                        break;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(index.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Now(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            try {
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(DateTime.Now.ToString("M.d.yyyy"), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Date, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Replace(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 3) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Replace[] Operand Function requires 3 text parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string newValue = Parameters[0].TokenName;
                string find = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName.ToString());
                string replace = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[2].TokenName.ToString());

                string final = newValue.Replace(find, replace);

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(final, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult PCase(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function PCase[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string final = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName.ToLower().Trim());
                string finished = final.Substring(0, 1).ToUpper() + final.Substring(1, final.Length - 1);

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem("\"" + finished + "\"", Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Not(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            Xap.Evaluation.Engine.Parser.TokenItem Result = null;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function Not[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // we can only take a Not of a boolean token
                if (Xap.Evaluation.Engine.Support.DataTypeCheck.IsBoolean(Parameters[0].TokenName) == true) {
                    bool temp = Parameters[0].TokenName_Boolean;
                    temp = !temp;

                    Result = new Xap.Evaluation.Engine.Parser.TokenItem(temp.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                } else {
                    operandResult = new OperandResult(null, "Error in operand function Not[]: Operand Function can only evaluate parameters that are boolean.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                //Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(null, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IsAllDigits(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Error in operand function IsAllDigits[]: Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // check the first parameter
                string checkString = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                bool allDigits = true; // assume all digits
                foreach (char c in checkString) {
                    if (Char.IsDigit(c) == false) {
                        allDigits = false;
                        break;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(allDigits.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult EndsWith(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "EndsWith[] Operand Function requires 2 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                string tempToken = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                // the last parameter must be an string
                string searchFor = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName);

                bool _retVal = false;
                _retVal = tempToken.EndsWith(searchFor);

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(_retVal.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult SqlRule(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count < 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "SqlRule[] Operand Function requires at least 3 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                //the second parameter should specify the database environment the is defined in the config file
                string env = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                if (env.TrimEnd().Length == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "The first parameter must be the name of the environment to execute in.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // check the first parameter.  it should give the name of the stored procedure to exexute.
                //it has to begin with spRule_ or it is invalid
                string spRule = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName);
                if (!spRule.Contains("spRule")) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "The second parameter must be the name of the stored procedure or a t-sql statement to execute.  Procedure name must begin with spRule_", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                object retVal = null;
                IXapDataProvider db = DbFactory.Instance.Db(env, spRule);
                try {
                    foreach (TokenItem t in Parameters) {
                        if (t.TokenName.Contains("@")) {
                            string[] args = System.Text.RegularExpressions.Regex.Split(t.TokenName, @"@");
                            foreach (string arg in args) {
                                string[] parms = System.Text.RegularExpressions.Regex.Split(arg, @"\|");
                                if (parms.Length == 2) {
                                    if (parms[0].StartsWith("'")) {
                                        parms[0] = parms[0].Remove(0, 1);
                                    }
                                    if (parms[1].EndsWith("'")) {
                                        parms[1] = parms[1].Remove(parms[1].Length - 1, 1);
                                    }
                                    db.AddParameter(DbFactory.Instance.DbParameter(parms[0], parms[1]));
                                }
                            }
                        }
                    }

                    retVal = db.ExecuteScalar();
                    if (retVal == null) {
                        retVal = "false";
                    } else if (retVal.ToString() == "0") {
                        retVal = "false";
                    } else if (string.IsNullOrEmpty(retVal.ToString())) {
                        retVal = "false";
                    } else if (retVal == "1") {
                        retVal = "true";
                    }
                } catch (Exception ex) {
                    XapLogger.Instance.Error(ex.ToString());
                    return null;
                }
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                if (retVal == "false") {
                    return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, false);
                }
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult SqlScalarFunction(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count < 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "SqlRule[] Operand Function requires at least 3 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                //the second parameter should specify the database environment the is defined in the config file
                string env = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                if (env.TrimEnd().Length == 0) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "The first parameter must be the name of the environment to execute in.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // check the first parameter.  it should give the name of the stored procedure to exexute.
                //it has to begin with spRule_ or it is invalid
                string spRule = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName);

                if (!spRule.Contains("spFunction")) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "The second parameter must be the name of the stored procedure or a t-sql statement to execute.  Procedure name must begin with spFunction_", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                object retVal = null;
                IXapDataProvider db = DbFactory.Instance.Db(env, spRule);
                try {
                    foreach (TokenItem t in Parameters) {
                        if (t.TokenName.Contains("@")) {
                            string[] args = System.Text.RegularExpressions.Regex.Split(t.TokenName, @"@");
                            foreach (string arg in args) {
                                string[] parms = System.Text.RegularExpressions.Regex.Split(arg, @"\|");
                                if (parms.Length == 2) {
                                    if (parms[0].StartsWith("'")) {
                                        parms[0] = parms[0].Remove(0, 1);
                                    }
                                    if (parms[1].EndsWith("'")) {
                                        parms[1] = parms[1].Remove(parms[1].Length - 1, 1);
                                    }
                                    db.AddParameter(DbFactory.Instance.DbParameter(parms[0], parms[1]));
                                }
                            }
                        }
                    }

                    retVal = db.ExecuteScalar();
                } catch (Exception ex) {
                    return null;
                }
                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);

                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult ListContains(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count <= 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "ListContains[] Operand Function requires at least 3 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                bool retVal = false;

                //get the string delimiter
                string delimiter = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                if (delimiter == "|") {
                    delimiter = @"\|";
                }

                // get the string to be used as the base for comparison
                string[] baseString = Regex.Split(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName), delimiter);

                //get the string to be 
                string[] inputString = Regex.Split(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[2].TokenName), delimiter);

                foreach (string s1 in inputString) {
                    foreach (string s2 in baseString) {
                        if (s1.ToUpper() == s2.ToUpper()) {
                            retVal = true;
                            break;
                        }
                    }
                    if (retVal == true) {
                        break;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal.ToString().ToLower(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                if (retVal == false) {
                    return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, false);
                }
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult ToInt32(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                if (Parameters.Count != 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "ToInt32[] Operand Function requires 1 parameter.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the token from the first parameter         
                string tmp = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);

                int outputInt;
                if (!int.TryParse(tmp, out outputInt)) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "ToInt32[] parameter contains a non-valid integer", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(tmp, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Int, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult SelectCase(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            try {
                //SelectCase[<FIELD_6>,M=Red|N=Blue]
                if (Parameters.Count < 2) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "SelectCase[]: Operand Function requires at least 2 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                string baseValue = Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[0].TokenName);
                string[] values = Regex.Split(Xap.Evaluation.Engine.Support.DataTypeCheck.RemoveTextQuotes(Parameters[1].TokenName), @"\|");
                string retVal = string.Empty;

                foreach (string value in values) {
                    string[] possbileValues = Regex.Split(value, @"\=");
                    if (possbileValues[0].ToUpper() == baseValue.ToUpper()) {
                        retVal = possbileValues[1];
                        break;
                    }
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal, Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_String, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult Requal(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            bool retVal = false;
            try {
                if (Parameters.Count <= 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "Requal[] Operand Function requires at least 2 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the search item
                string value1 = Parameters[0].TokenName;
                string value2 = Parameters[1].TokenName;
                if (value1 == value2) {
                    retVal = true;
                }

                Xap.Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Xap.Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }

        public Xap.Evaluation.Engine.Support.OperandResult IsValidDate(Xap.Evaluation.Engine.Parser.TokenItems Parameters) {
            OperandResult operandResult;
            bool retVal = false;
            try {
                if (Parameters.Count < 1) {
                    operandResult = new Xap.Evaluation.Engine.Support.OperandResult(null, "IsValidDate[] Operand Function requires at least 1 parameters.", false);
                    XapLogger.Instance.Error(operandResult.ErrorMessage);
                    return operandResult;
                }

                // get the search item
                string value1 = Parameters[0].TokenName;
                try {
                    DateTime outDate;
                    if (DateTime.TryParse(value1, out outDate)) {
                        retVal = true;
                    }
                } catch {
                    // throw new Exception("Error converting " + str + " to DateTime");
                }

                Evaluation.Engine.Parser.TokenItem Result = new Xap.Evaluation.Engine.Parser.TokenItem(retVal.ToString(), Xap.Evaluation.Engine.Parser.TokenType.Token_Operand, Xap.Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, false);
                return new Evaluation.Engine.Support.OperandResult(Result, String.Empty, true);
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.ToString());
                return null;
            }
        }
        #endregion

        #region "Operand Helpers"
        private bool IsHoliday(DateTime date, string[] _holidays) {
            string dateCheck = date.ToString("MM/dd/yyyy");
            return Array.Exists(_holidays, element => element == dateCheck);
        }

        private bool IsWeekEnd(DateTime date) {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }
        #endregion
    }
}
