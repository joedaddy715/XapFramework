using System;
using System.Reflection;
using Xap.Evaluation.Engine.Cache;
using Xap.Infrastructure.Logging;

//http://www.codeproject.com/Articles/26314/Evaluation-Engine
//
namespace Xap.Evaluation.Engine.Evaluate {

    public class Evaluator {
        #region Local Variables

        private Parser.Token token;

        private double tokenEvalTime = 0;

        #endregion

        #region Public Constructors

        public Evaluator(Parser.Token Tokens) {
            token = Tokens;
        }

        #endregion

        #region Public Properties

        public double TokenEvalTime {
            get {
                return tokenEvalTime;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// This new evaluate function includes support to assignment and short circuit of the IIF[] operand function
        /// </summary>
        /// <param name="RPNQueue"></param>
        /// <param name="sValue"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public bool Evaluate(Support.ExQueue<Parser.TokenItem> RPNQueue, out string sValue, out string ErrorMsg) {
            // initialize the outgoing variable
            ErrorMsg = "";
            sValue = "";

            // reset the results in the token
            token.LastEvaluationResult = "";

            // create a stop watch to time the evaluation

            //System.Diagnostics.Stopwatch evalTime = System.Diagnostics.Stopwatch.StartNew();

            // make sure the otkens are valid
            if (token.AnyErrors == true) {
                // the token already has an error, return the token error as the evaluator error message
                ErrorMsg = token.LastErrorMessage;
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }


            // create the evaluation stack
            Support.ExStack<Parser.TokenItem> eval = new Support.ExStack<Parser.TokenItem>(token.TokenItems.Count);

            // start looping through the tokens
            int count = RPNQueue.Count;
            int index = 0;  // the index of the curent token item in the rpn queue


            while (index < count) {
                // get the next token item
                Parser.TokenItem item = RPNQueue[index];
                index++;

                if (item.TokenDataType == Evaluation.Engine.Parser.TokenDataType.Token_DataType_Variable) {
                    #region Token_DataType_Variable

                    // determine if we need to assign the variable represented by the token
                    // or the rule syntax is doing the assignment
                    if (item.WillBeAssigned == false) {
                        // The rule syntax is not doing the assignment, we are doing it.
                        // lookup the value of the variable and push it onto the evaluation stack
                        if (token.Variables.VariableExists(item.TokenName) == true) {
                            // the variable exists, push it on the stack
                            eval.Push(new Parser.TokenItem(token.Variables[item.TokenName].VariableValue, Parser.TokenType.Token_Operand, item.InOperandFunction));
                        } else {
                            // the variable does not exist...push an empty string on the stack
                            eval.Push(new Parser.TokenItem("", Parser.TokenType.Token_Operand, item.InOperandFunction));
                        }
                    } else {
                        // the rule syntax is doing the assignment, add the token item to the evaluation stack
                        eval.Push(item);
                    }

                    #endregion
                } else if (item.TokenType == Evaluation.Engine.Parser.TokenType.Token_Operator) {
                    #region Token_Operator

                    // pop 2 items off the stack and perform the operation
                    // push the result back onto the evaluation stack

                    Parser.TokenItem rightOperand = null;
                    Parser.TokenItem leftOperand = null;
                    try {
                        if (eval.Count > 0) rightOperand = eval.Pop();
                        if (eval.Count > 0) leftOperand = eval.Pop();
                    } catch (Exception err) {
                        ErrorMsg = "Error in Evaluation.Engine.Evaluator.Evaluate() while popping 2 tokens for an operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    // double check that we got the tokens before we evaluate
                    if (rightOperand == null) {
                        ErrorMsg = "Failed to evaluate the rule expression: The right operand token is null: There may be an issue with the rule syntax.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    if (leftOperand == null) {
                        ErrorMsg = "Failed to evaluate the rule expression: The left operand token is null: There may be an issue with the rule syntax.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    // process the operator
                    try {
                        Parser.TokenItem result = null;
                        if (EvaluateTokens(leftOperand, rightOperand, item, out result, out ErrorMsg) == false)
                            return false;
                        else {
                            // double check that we got a result
                            if (result == null) {
                                ErrorMsg = "Failed to evaluate the rule expression: The result of an operator is null: There may be an issue with the rule syntax.";
                                XapLogger.Instance.Error(ErrorMsg);
                                return false;
                            } else {
                                eval.Push(result);
                            }
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the rule expression: The result of an operator threw an error: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }


                    #endregion
                } else if (item.TokenType == Evaluation.Engine.Parser.TokenType.Token_Operand_Function_Stop) {
                    #region Token_Operand_Function_Stop

                    // find the start of the function by popping off items
                    // evaluate the function and push the result back onto the evaluation stack

                    // start popping items from the evaluation stack
                    // until we get the start of the of the operand function
                    int evalCount = eval.Count;
                    Parser.TokenItems parameters = new Parser.TokenItems(token);

                    try {
                        for (int j = 0; j < evalCount; j++) {
                            Parser.TokenItem opItem = eval.Pop();

                            if (opItem.TokenType == Evaluation.Engine.Parser.TokenType.Token_Operand_Function_Start) {
                                // we found the start of the operand function; let's evaluate it

                                Parser.TokenItem result = null;
                                EvaluateOperandFunction(opItem, parameters, out result, out ErrorMsg);
                                //if (EvaluateOperandFunction(opItem, parameters, out result, out ErrorMsg) == false)
                                //     return false;
                                //else
                                //{
                                // make sure we got a result
                                if (result == null) {
                                    ErrorMsg = "Failed to evaluate the rule expression: The result of an operand function is null: There may be an issue with the rule syntax.";
                                    XapLogger.Instance.Error(ErrorMsg);
                                    return false;
                                } else
                                    eval.Push(result);
                                //}
                                break;
                            } else if (opItem.TokenType != Evaluation.Engine.Parser.TokenType.Token_Operand_Function_Delimiter) {
                                // we have a parameter to the operand function
                                parameters.AddToFront(opItem);
                            }
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the rule expression: The evaluation of an operand function threw an error: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    #endregion
                } else if (item.TokenType == Evaluation.Engine.Parser.TokenType.Token_Assignemt_Start) {
                    #region Token_Assignment_Start

                    // assign the value to the variable

                    // pop 2 items off the stack - save the value into the variable

                    Parser.TokenItem rightOperand = null;
                    Parser.TokenItem leftOperand = null;
                    try {
                        if (eval.Count > 0) rightOperand = eval.Pop();
                        if (eval.Count > 0) leftOperand = eval.Pop();
                    } catch (Exception err) {
                        ErrorMsg = "Error in Evaluation.Engine.Evaluator.Evaluate() while popping 2 tokens for an operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    // double check that we got the tokens before we evaluate
                    if (rightOperand == null) {
                        ErrorMsg = "Failed to evaluate the rule expression: The right operand token is null: There may be an issue with the rule syntax.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    if (leftOperand == null) {
                        ErrorMsg = "Failed to evaluate the rule expression: The left operand token is null: There may be an issue with the rule syntax.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    // look for the variable and assign the value to it
                    if (token.Variables.VariableExists(leftOperand.TokenName) == true) {
                        // the variable exists, push it on the stack
                        token.Variables[leftOperand.TokenName].VariableValue = rightOperand.TokenName;
                    } else {
                        // failed to find the variable....this is an error
                        ErrorMsg = "Failed to evaluate the rule expression: Failed to find the variable '" + leftOperand.TokenName + "' for the assignment.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }




                    #endregion
                } else if (item.TokenType == Evaluation.Engine.Parser.TokenType.Token_Operand_Function_Start) {
                    #region New Short Circuit Code

                    // we are only short circuiting the IIF[] operand function
                    if (item.TokenName.Trim().ToLower() != "iif[") {
                        // add the token to the evaluation stack
                        eval.Push(item);
                    } else {
                        // we found the iif statement.

                        // see if the iff[] operand function allows for short circuiting
                        if (item.CanShortCircuit == false) {
                            // no short circuiting, add it to the evaluation stack
                            eval.Push(item);
                        } else {
                            ////////////////////////////////////////////////
                            // We can short circuit this iif[] statement  //
                            ////////////////////////////////////////////////

                            Parser.TokenItem result = item.ShortCircuit.Evaluate(out ErrorMsg);

                            if (result == null) {
                                // there was an error doing the short circuit
                                return false;
                            } else {
                                // we successfully did the short circuit
                                eval.Push(result);

                                // increment the index so we skip the ] which should be the next token
                                index++;
                            }

                        }
                    }

                    #endregion
                } else {
                    // push the item on the evaluation stack
                    eval.Push(item);
                }
            }

            if (eval.Count == 1) {
                // just 1 item on the stack; should be our answer
                try {
                    Parser.TokenItem final = eval.Pop();
                    sValue = final.TokenName;

                    // set the results in the token
                    token.LastEvaluationResult = sValue;
                } catch (Exception err) {
                    ErrorMsg = "Failed to evaluate the rule expression after all the tokens have been considered: " + err.Message;
                    XapLogger.Instance.Error(ErrorMsg);
                    return false;
                }
            } else if (eval.Count == 0) {
                // there is no result in the evaluation stack because it my have been assigned
                // do nothing here
            } else {
                ErrorMsg = "Invalid Rule Syntax";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }


            // stop the timer
            //evalTime.Stop();

            //tokenEvalTime = evalTime.Elapsed.TotalMilliseconds;
            // token.LastEvaluationTime = tokenEvalTime; // set this evaluation time in the token object.
            return true;

        }

        /// <summary>
        /// This new evaluate function includes support to assignment and short circuit of the IIF[] operand function
        /// </summary>
        /// <param name="sValue"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public bool Evaluate(out string sValue, out string ErrorMsg) {
            return Evaluate(token.RPNQueue, out sValue, out ErrorMsg);
        }

        #endregion

        #region Private Methods

        private bool EvaluateTokens(Parser.TokenItem LeftOperand, Parser.TokenItem RightOperand, Parser.TokenItem Operator, out Parser.TokenItem Result, out string ErrorMsg) {
            // intitialize the outgoing variables
            Result = null;
            ErrorMsg = "";

            // local variables
            double dResult = 0;
            bool boolResult = false;

            // validate the parameters
            if (LeftOperand == null) {
                ErrorMsg = "Failed to evaluate the operator: The left token is null.";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }

            if (RightOperand == null) {
                ErrorMsg = "Failed to evaluate the operator: The right token is null.";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }

            if (Operator == null) {
                ErrorMsg = "Failed to evaluate the operator: The operator token is null.";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }


            switch (Operator.TokenName.Trim().ToLower()) {
                case "^":
                    #region Exponents

                    // Exponents require that both operands can be converted to doubles
                    try {
                        if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                            dResult = Math.Pow(LeftOperand.TokenName_Double, RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(dResult.ToString(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, LeftOperand.InOperandFunction);
                        } else {
                            ErrorMsg = "Syntax Error: Expecting numeric values for exponents.";
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the Exponent operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    break;
                #endregion

                case "*":
                    #region Multiplication

                    //  multiplication expects that the operands can be converted to doubles
                    try {
                        if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                            dResult = LeftOperand.TokenName_Double * RightOperand.TokenName_Double;
                            Result = new Parser.TokenItem(dResult.ToString(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, LeftOperand.InOperandFunction);
                        } else {
                            ErrorMsg = "Syntax Error: Expecting numeric values to multiply.";
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the Multiplication operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    break;
                #endregion

                case "/":
                    #region Division

                    // divison requires that both operators can be converted to doubles and the denominator is not 0

                    try {
                        if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                            double denominator = RightOperand.TokenName_Double;

                            if (denominator != 0) {
                                dResult = LeftOperand.TokenName_Double / denominator;
                                Result = new Parser.TokenItem(dResult.ToString(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, LeftOperand.InOperandFunction);
                            } else {
                                ErrorMsg = "Syntax Error: Division by zero.";
                                XapLogger.Instance.Error(ErrorMsg);
                                return false;
                            }
                        } else {
                            ErrorMsg = "Syntax Error: Expecting numeric values to divide.";
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the Division operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    break;
                #endregion

                case "%":
                    #region Modulus
                    try {
                        // modulus expects that both operators are numeric and the right operand is not zero
                        if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                            double denominator = RightOperand.TokenName_Double;

                            if (denominator != 0) {

                                dResult = LeftOperand.TokenName_Double % RightOperand.TokenName_Double;
                                Result = new Parser.TokenItem(dResult.ToString(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, LeftOperand.InOperandFunction);
                            } else {
                                ErrorMsg = "Syntax Error: Modulus by zero.";
                                XapLogger.Instance.Error(ErrorMsg);
                                return false;
                            }
                        } else {
                            ErrorMsg = "Syntax Error: Expecting numeric values to modulus.";
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the Modulus operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    break;
                #endregion

                case "+":
                    #region Addition

                    try {
                        // addition only works on numeric operands
                        if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                            dResult = LeftOperand.TokenName_Double + RightOperand.TokenName_Double;
                            Result = new Parser.TokenItem(dResult.ToString(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, LeftOperand.InOperandFunction);
                        } else {
                            ErrorMsg = "Syntax Error: Expecting numeric values to add.";
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the Addition operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    break;
                #endregion

                case "-":
                    #region Subtraction
                    try {
                        // subtraction only works on numeric operands
                        if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                            dResult = LeftOperand.TokenName_Double - RightOperand.TokenName_Double;
                            Result = new Parser.TokenItem(dResult.ToString(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Double, LeftOperand.InOperandFunction);
                        } else {
                            ErrorMsg = "Syntax Error: Expecting numeric values to subtract.";
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } catch (Exception err) {
                        ErrorMsg = "Failed to evaluate the Subtraction operator: " + err.Message;
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }
                    break;
                #endregion

                case "<":
                    #region Less Than
                    if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                        try {
                            // do a numeric comparison
                            boolResult = (LeftOperand.TokenName_Double < RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Less Than operator on double operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsDate(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDate(RightOperand.TokenName) == true)) {
                        try {
                            // do a date comparison
                            TimeSpan ts = LeftOperand.TokenName_DateTime.Subtract(RightOperand.TokenName_DateTime);
                            boolResult = (ts.TotalDays < 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Less Than operator on date operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        try {
                            // do a string comparison
                            string lText = Support.DataTypeCheck.RemoveTextQuotes(LeftOperand.TokenName);
                            string rText = Support.DataTypeCheck.RemoveTextQuotes(RightOperand.TokenName);

                            boolResult = (lText.CompareTo(rText) < 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Less Than operator on string operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    }

                    break;
                #endregion

                case "<=":
                    #region Less Than Equal To

                    if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                        try {
                            // do a numeric comparison
                            boolResult = (LeftOperand.TokenName_Double <= RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Less Than or Equal to operator on double operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsDate(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDate(RightOperand.TokenName) == true)) {
                        try {
                            // do a date comparison
                            TimeSpan ts = LeftOperand.TokenName_DateTime.Subtract(RightOperand.TokenName_DateTime);
                            boolResult = (ts.TotalDays <= 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Less Than or Equal to operator on date operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        try {
                            // do a string comparison
                            string lText = Support.DataTypeCheck.RemoveTextQuotes(LeftOperand.TokenName);
                            string rText = Support.DataTypeCheck.RemoveTextQuotes(RightOperand.TokenName);

                            boolResult = (lText.CompareTo(rText) <= 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Less Than or Equal to operator on string operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    }
                    break;
                #endregion

                case ">":
                    #region Greater than

                    if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                        try {
                            // do a numeric comparison
                            boolResult = (LeftOperand.TokenName_Double > RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Greater Than to operator on double operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsDate(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDate(RightOperand.TokenName) == true)) {
                        try {
                            // do a date comparison
                            TimeSpan ts = LeftOperand.TokenName_DateTime.Subtract(RightOperand.TokenName_DateTime);
                            boolResult = (ts.TotalDays > 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Greater Than to operator on date operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        try {
                            // do a string comparison
                            string lText = Support.DataTypeCheck.RemoveTextQuotes(LeftOperand.TokenName);
                            string rText = Support.DataTypeCheck.RemoveTextQuotes(RightOperand.TokenName);

                            boolResult = (lText.CompareTo(rText) > 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Greater Than to operator on string operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    }
                    break;
                #endregion

                case ">=":
                    #region Greater than equal to
                    if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                        try {
                            // do a numeric comparison
                            boolResult = (LeftOperand.TokenName_Double >= RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Greater Than or Equal to operator on double operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsDate(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDate(RightOperand.TokenName) == true)) {
                        try {
                            // do a date comparison
                            TimeSpan ts = LeftOperand.TokenName_DateTime.Subtract(RightOperand.TokenName_DateTime);
                            boolResult = (ts.TotalDays >= 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Greater Than or Equal to operator on date operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        try {
                            // do a string comparison
                            string lText = Support.DataTypeCheck.RemoveTextQuotes(LeftOperand.TokenName);
                            string rText = Support.DataTypeCheck.RemoveTextQuotes(RightOperand.TokenName);

                            boolResult = (lText.CompareTo(rText) >= 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Greater Than or Equal to operator on string operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    }
                    break;
                #endregion

                case "<>":
                    #region Not equal to
                    if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                        try {
                            // do a numeric comparison
                            boolResult = (LeftOperand.TokenName_Double != RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Not Equal To operator on double operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsDate(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDate(RightOperand.TokenName) == true)) {
                        try {
                            // do a date comparison
                            TimeSpan ts = LeftOperand.TokenName_DateTime.Subtract(RightOperand.TokenName_DateTime);
                            boolResult = (ts.TotalDays != 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Not Equal To operator on date operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsBoolean(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsBoolean(RightOperand.TokenName) == true)) {
                        try {
                            boolResult = (LeftOperand.TokenName_Boolean != RightOperand.TokenName_Boolean);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Not Equal To operator on boolean operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        try {
                            // do a string comparison
                            string lText = Support.DataTypeCheck.RemoveTextQuotes(LeftOperand.TokenName);
                            string rText = Support.DataTypeCheck.RemoveTextQuotes(RightOperand.TokenName);

                            boolResult = (lText.Equals(rText) == false);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Not Equal To operator on string operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    }
                    break;
                #endregion

                case "=":
                    #region Equal to

                    if ((Support.DataTypeCheck.IsDouble(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDouble(RightOperand.TokenName) == true)) {
                        try {
                            // do a numeric comparison
                            boolResult = (LeftOperand.TokenName_Double == RightOperand.TokenName_Double);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Equal To operator on double operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsDate(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsDate(RightOperand.TokenName) == true)) {
                        try {
                            // do a date comparison
                            TimeSpan ts = LeftOperand.TokenName_DateTime.Subtract(RightOperand.TokenName_DateTime);
                            boolResult = (ts.TotalDays == 0);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Equal To operator on date operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else if ((Support.DataTypeCheck.IsBoolean(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsBoolean(RightOperand.TokenName) == true)) {
                        try {
                            boolResult = (LeftOperand.TokenName_Boolean == RightOperand.TokenName_Boolean);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Equal To operator on boolean operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        try {
                            // do a string comparison
                            string lText = Support.DataTypeCheck.RemoveTextQuotes(LeftOperand.TokenName);
                            string rText = Support.DataTypeCheck.RemoveTextQuotes(RightOperand.TokenName);

                            boolResult = lText.Equals(rText);
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the Equal To operator on stirng operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    }
                    break;
                #endregion

                case "and":
                    #region and

                    // the and operator must be performed on boolean operators
                    if ((Support.DataTypeCheck.IsBoolean(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsBoolean(RightOperand.TokenName) == true)) {
                        try {
                            boolResult = LeftOperand.TokenName_Boolean && RightOperand.TokenName_Boolean;
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the AND operator on boolean operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        ErrorMsg = "Syntax Error: Expecting boolean operands to AND.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    break;

                #endregion

                case "or":
                    #region or

                    if ((Support.DataTypeCheck.IsBoolean(LeftOperand.TokenName) == true) && (Support.DataTypeCheck.IsBoolean(RightOperand.TokenName) == true)) {
                        try {
                            boolResult = LeftOperand.TokenName_Boolean || RightOperand.TokenName_Boolean;
                            Result = new Parser.TokenItem(boolResult.ToString().ToLower(), Parser.TokenType.Token_Operand, Evaluation.Engine.Parser.TokenDataType.Token_DataType_Boolean, LeftOperand.InOperandFunction);
                        } catch (Exception err) {
                            ErrorMsg = "Failed to evaluate the OR operator on boolean operands: " + err.Message;
                            XapLogger.Instance.Error(ErrorMsg);
                            return false;
                        }
                    } else {
                        ErrorMsg = "Syntax Error: Expecting boolean operands to OR.";
                        XapLogger.Instance.Error(ErrorMsg);
                        return false;
                    }

                    break;

                #endregion

                default:
                    #region Unknown Operator

                    ErrorMsg = "Failed to evaluate the operator: The operator token is null.";
                    XapLogger.Instance.Error(ErrorMsg);
                    return false;

                    #endregion
                    //break;
            }

            if (Result == null) {
                ErrorMsg = "Syntax Error: Failed to evaluate the expression.";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            } else
                return true;
        }

        private bool EvaluateOperandFunction(Parser.TokenItem OperandFunction, Parser.TokenItems Parameters, out Parser.TokenItem Result, out string ErrorMsg) {
            // intitialize the outgoing variables
            Result = null;  // assume a failure by setting the result to null
            ErrorMsg = "";

            // local variables
            bool success = true;

            // validate the parameters
            if (OperandFunction == null) {
                ErrorMsg = "Failed to evaluate the operand function: The operand function token is null.";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }

            if (Parameters == null) {
                ErrorMsg = "Failed to evaluate the operand function: The parameters collection is null.";
                XapLogger.Instance.Error(ErrorMsg);
                return false;
            }

            ScriptCacheManager.Instance.ExtractScriptOperands();

            foreach (ScriptOperand operand in ScriptCacheManager.Instance.GetScriptOperands()) {
                foreach (MethodInfo method in operand.GetEngineOperandMethods()) {
                    if (method.Name + "[" == OperandFunction.TokenName) {
                        //execute the method and exit loop
                        try {
                            object[] p = new object[1];
                            p[0] = Parameters;
                            Support.OperandResult _retVal = (Support.OperandResult)method.Invoke(operand.EngineOperand, p);
                            success = _retVal.Success;
                            Result = _retVal.Result;
                        } catch (Exception ex) {
                            XapLogger.Instance.Error($"Failed to evaluate the operand function {OperandFunction.TokenName.Trim()}");
                            ErrorMsg = "Failed to evaluate the operand function " + OperandFunction.TokenName.Trim() + ": " + ex.Message;
                            success = false;
                        }
                    }
                }
            }
            return success;
        }
        #endregion
    }
}

